using System;
using System.Collections;
using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
    public enum weaponSlot
    {
        Primary = 0,　   //Primary：ライフル
        Secondary = 1,   //Secondary：ピストル
        Tertiary = 2,    //Tertiary：ロケットランチャー
        Quaternary = 3, //quaternary：スナイパーライフル
        Length,
    }

    [SerializeField] private Transform[] weaponSlots;
    [SerializeField] private Transform weaponRightGrip;
    [SerializeField] private Transform weaponLeftGrip;
    [SerializeField] private LayerMask excludeTarget;
    [SerializeField] private Animator rigController;

    private Transform crosshairTarget;

    public bool isChangingWeapon { get; private set; }

    [HideInInspector] public bool isHolstered = false;

    [SerializeField] WeaponAnimationEvents animationEvents;

    //入力
    private PlayerInput playerInput;

    //コンポネント
    private PlayerAiming characterAiming;
    private RaycastWeapon[] equipped_Weapons = new RaycastWeapon[(int)weaponSlot.Length];
    private ReloadWeapon reloadWeapon;
    private GrenadeController grController;    

    private int unarmedParam = Animator.StringToHash("weapon_Unarmed");

    private int activeWeaponIndex;
    private Vector3 aimPoint;

    private weaponSlot[] duplicationSlots = { weaponSlot.Tertiary, weaponSlot.Quaternary };

    //コンポネント取得
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        characterAiming = GetComponent<PlayerAiming>();
        reloadWeapon = GetComponent<ReloadWeapon>();
        grController = GetComponent<GrenadeController>();
        animationEvents.WeaponAnimationEvent.AddListener(OnAnimationEvent);

        crosshairTarget = GameObject.FindWithTag("CrossHairTarget").transform;
        if (crosshairTarget == null) { Debug.Log("CrossHairTarget is Null"); }
    }

    private void Start()
    {
        //武器を所持しているか確認
        RaycastWeapon exisitingWeapon = GetComponentInChildren<RaycastWeapon>();
        if (exisitingWeapon)
        {
            Equip(exisitingWeapon);//装備
        }
    }

    private void OnEnable()
    {
        SetDefaultState();
    }

    //初期の状態にする（primary = null secondary = pistol　Tertiary = RocketLauncher Quaternary = SniperRifle）
    private void SetDefaultState()
    {
        isHolstered = true;
        isChangingWeapon = false;

        rigController.SetBool("holster_weapon", true);
        rigController.Play(unarmedParam);

        activeWeaponIndex = (int)weaponSlot.Secondary;
        rigController.SetInteger("weapon_index", activeWeaponIndex);

        //所持しているすべての（拳銃を除いて）武器を破棄
        var weapon = weaponSlots[(int)weaponSlot.Primary].GetComponentInChildren<RaycastWeapon>();
        if (weapon) Destroy(weapon.gameObject);
        UIManager.Instance?.UpdateWeaponSlot((int)weaponSlot.Primary, false);
        
        weapon = weaponSlots[(int)weaponSlot.Tertiary].GetComponentInChildren<RaycastWeapon>();
        if (weapon) Destroy(weapon.gameObject);
        UIManager.Instance?.UpdateWeaponSlot((int)weaponSlot.Tertiary, false);

        weapon = weaponSlots[(int)weaponSlot.Quaternary].GetComponentInChildren<RaycastWeapon>();
        if (weapon) Destroy(weapon.gameObject);
        UIManager.Instance?.UpdateWeaponSlot((int)weaponSlot.Quaternary, false);

        UIManager.Instance.UpdateWeaponSlotImage((int)weaponSlot.Secondary);

    }

    //標的オブジェクトを更新する
    private void UpdateAimTarget()
    {
        RaycastHit hit;
        Camera playerCamera = Camera.main;
        RaycastWeapon weapon = GetActiveWeapon();
        if (!weapon)
        {
            return;
        }
        Vector3 firePosition = weapon.raycastOrigin.transform.position;
        var ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); //画面中央にRay 

        if (Physics.Raycast(ray, out hit, 200.0f, ~excludeTarget)) 
        {
            //excludeTargetを除く
            aimPoint = hit.point; //画面中央からのRayに当たった的 (FPS)

            //的とプレイヤーキャラクタの間に何かのオブジェクトがあったら標的をそのオブジェクトに変える (TPS)
            if (Physics.Linecast(firePosition, hit.point, out hit, ~excludeTarget)) 
            {
                aimPoint = hit.point;
            }
        }
        else
        {
            aimPoint = playerCamera.transform.position + playerCamera.transform.forward * 200.0f; //fire distance < 수정예정 
        }
    }

    //現在使用している武器取得
    RaycastWeapon GetWeapon(int index)
    {
        if (index < 0 || index >= equipped_Weapons.Length)
        {
            return null;
        }
        return equipped_Weapons[index];
    }

    //UI
    private void UpdateUI() 
    {
        var weapon = GetActiveWeapon();
        if (weapon == null) return;
        if (UIManager.Instance == null) return;
        // 残弾数
        UIManager.Instance.UpdateAmmoText(weapon.magAmmo, weapon.ammoRemain, weapon.isInifinty);
        // クロスヘアのサイズ
        UIManager.Instance.SetActiveCrosshair(!isHolstered);
        //クロスヘアを的の位置に表示させる
        UIManager.Instance.UpdateCrossHairPosition(aimPoint); 
    }

    void FixedUpdate()
    {
        if (UIManager.Instance.isPause) return;　//Pause 
        //走っているか
        bool notSprinting = rigController.GetCurrentAnimatorStateInfo(2).shortNameHash 
                == Animator.StringToHash("notSprinting");
        //リロードしているか
        bool isReloading = reloadWeapon.isReloading;
        
        if (isReloading || !notSprinting) return;

        var weapon = GetWeapon(activeWeaponIndex);
        if (weapon && !isHolstered)　//武器を所持している＆取り出している
        {
            //武器の処理
            weapon.UpdateWeapon(Time.deltaTime);
            UpdateAimTarget();
        }

        UpdateWeaponControl();

        UpdateUI();
    }

    public void ThrowGrenade()
    {
        if (!grController)
        {
            Debug.Log("grController variable has not been assigned");
            return;
        }
        grController.ThrowGrenade();
    }

    public bool IsFiring()
    {
        RaycastWeapon currentWeapon = GetActiveWeapon();
        if (!currentWeapon)
        {
            return false;
        }
        return currentWeapon.isFiring;
    }

    //武器を装備する
    public void Equip(RaycastWeapon newWeapon)
    {
        RaycastWeapon weapon;

        int weaponSlotIndex = (int)newWeapon.weaponSlot; //装備する武器のindexを読み込む
        var isDuplication = Array.Exists(duplicationSlots, ws => ws == newWeapon.weaponSlot);
        
        if (isDuplication)
        {
            foreach (var slot in duplicationSlots)
            {
                weapon = GetWeapon((int)slot); //すでに他の武器を所持しているか確認
                if (weapon) 
                {
                    Destroy(weapon.gameObject);
                    UIManager.Instance?.UpdateWeaponSlot((int)weapon.weaponSlot, false);
                } // あったら破棄
            }
        }
        else 
        {
            weapon = GetWeapon(weaponSlotIndex); 
            if (weapon) 
            {
                Destroy(weapon.gameObject);
                UIManager.Instance?.UpdateWeaponSlot((int)weapon.weaponSlot, false);
            }
        }

        //装備、パラメータ設定
        weapon = newWeapon;
        weapon.SetHolder(gameObject, playerInput);
        weapon.raycastDestination = crosshairTarget; 
        weapon.recoil.characterAiming = characterAiming;
        weapon.recoil.rigController = rigController;
        weapon.transform.SetParent(weaponSlots[weaponSlotIndex], false); 
        equipped_Weapons[weaponSlotIndex] = weapon;

        //SetActiveWeapon(newWeapon.weaponSlot); 
        UIManager.Instance?.UpdateWeaponSlot(weaponSlotIndex, true);
    }

    //武器選択処理 
    private void UpdateWeaponControl()
    {
        if (isChangingWeapon) { return; }
        //武器を取り出す
        if (playerInput.Toggle)
        {
            ToggleActiveWeapon();
        }
        if (playerInput.Alpha1 && equipped_Weapons[(int)weaponSlot.Secondary] != null)
        {
            SetActiveWeapon(weaponSlot.Secondary);
        }
        if (playerInput.Alpha2 && equipped_Weapons[(int)weaponSlot.Primary] != null)
        {
            SetActiveWeapon(weaponSlot.Primary);
        }
        if (playerInput.Alpha3 && equipped_Weapons[(int)weaponSlot.Tertiary] != null)
        {
            SetActiveWeapon(weaponSlot.Tertiary);
        }
        if (playerInput.Alpha4 && equipped_Weapons[(int)weaponSlot.Quaternary] != null)
        {
            SetActiveWeapon(weaponSlot.Quaternary);
        }
        //グレネードを投げる
        if (playerInput.Grenade && grController.isAvailable && !isHolstered)
        {
            rigController.SetTrigger("throw_grenade");
        }
    }

    //primary < > secondary 変更処理
    void ToggleActiveWeapon()
    {
        switch (activeWeaponIndex)
        {
            case (int)weaponSlot.Primary :
                SetActiveWeapon(weaponSlot.Primary);
                break;             
            case (int)weaponSlot.Secondary:
                SetActiveWeapon(weaponSlot.Secondary);
                break;
            case (int)weaponSlot.Tertiary:
                SetActiveWeapon(weaponSlot.Tertiary);
                break;
            case (int)weaponSlot.Quaternary:
                SetActiveWeapon(weaponSlot.Quaternary);
                break;
            default:
                break;
        }
    }

    void SetActiveWeapon(weaponSlot weaponslot)
    {
        int holsterIndex = activeWeaponIndex; // 
        int activateIndex = (int)weaponslot; //

        if (holsterIndex == activateIndex)
        {
            if (isHolstered)
            {
                holsterIndex = -1;
            }
            else
            {
                activateIndex = -1;
            }
        }
        StartCoroutine(SwitchWeapon(holsterIndex, activateIndex));
    }
    IEnumerator SwitchWeapon(int holsterindex, int activateIndex)
    {
        rigController.SetInteger("weapon_index", activateIndex); 
        yield return StartCoroutine(HolsterWeapon(holsterindex));
        yield return StartCoroutine(ActivateWeapon(activateIndex));
        activeWeaponIndex = activateIndex >= 0 ? activateIndex : activeWeaponIndex;
    }
    IEnumerator HolsterWeapon(int index)
    {
        isChangingWeapon = true;
        isHolstered = true;
        var weapon = GetWeapon(index);
        if (weapon)
        {
            rigController.SetBool("holster_weapon", true);
            do
            {
                yield return new WaitForSeconds(0.02f);
            } while (rigController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
        }
        //UIManager.Instance.UpdateWeaponSlotImage(index, false);
        isChangingWeapon = false;
    }
    IEnumerator ActivateWeapon(int index)
    {
        isChangingWeapon = true;
        var weapon = GetWeapon(index);
        if (weapon)
        {
            rigController.Play("equip_" + weapon.weaponType);
            rigController.SetBool("holster_weapon", false);
            do
            {
                yield return new WaitForSeconds(0.02f);
            } while (rigController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
            isHolstered = false;
        }
        UIManager.Instance.UpdateWeaponSlotImage(index);
        isChangingWeapon = false;
    }

    //使用している武器を取得
    public RaycastWeapon GetActiveWeapon()
    {
        return GetWeapon(activeWeaponIndex);
    }

    public RaycastWeapon GetPrimaryWeapon()
    {
        var primary = GetWeapon(0);
        if (primary)
        {
            return primary;
        }
        return null;
    }

    private void OnAnimationEvent(string evenName)
    {
        switch (evenName)
        {
            case "throw_Grenade":
                ThrowGrenade();
                break;

            default:
                break;
        }
    }
}
