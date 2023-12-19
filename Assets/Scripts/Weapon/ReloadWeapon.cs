using System.Collections;
using UnityEngine;

public class ReloadWeapon : MonoBehaviour
{
    //入力
    private PlayerInput playerInput;


    //アニメーションリギング
    [SerializeField] Animator rigController;
    [SerializeField] WeaponAnimationEvents animationEvents;

    public Transform leftHand;
    public bool isReloading { get; private set; }

    private const float timeBetNextAction = 0.6f;

    ActiveWeapon activeWeapon;

    GameObject magazineHand;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        activeWeapon = GetComponent<ActiveWeapon>(); 
        animationEvents.WeaponAnimationEvent.AddListener(OnAnimationEvent);  
    }

    private void OnEnable()
    {
        isReloading = false;
    }

    void Update()
    {
        RaycastWeapon weapon = activeWeapon.GetActiveWeapon();
        if (weapon)
        {
            bool isChanging = activeWeapon.isChangingWeapon;
            bool isHolstered = activeWeapon.isHolstered;
            //りーろど可能な状態か確認
            if (playerInput.Reload && !isChanging && !isHolstered) // || weapon.ammoCount <= 0
            {
                if (weapon.reloadAvailable)
                {
                    StartReload();
                }
            }
        }
    }

    public void StartReload()
    {
        //リロードアニメーション再生
        rigController.SetTrigger("reload_weapon");
        isReloading = true;
    }

    //リロードアニメーションからの呼出　
    void OnAnimationEvent(string evenName)
    {
        switch (evenName)
        {
            case "detach_magazine":
                DetachMagazine();
                break;
            case "drop_magazine":
                DropMagazine();
                break;
            case "refill_magazine":
                RefillMagazine();
                break;
            case "attach_magazine":
                AttachMagazine();
                break;
            default:
                break;
        }
    }
    //弾倉の着脱、アニメーションによって順番に呼出
    void DetachMagazine()
    {
        RaycastWeapon weapon = activeWeapon.GetActiveWeapon();
        if (magazineHand == null)
        {
            magazineHand = Instantiate(weapon.magazine, leftHand, true);
        }
        weapon.magazine.SetActive(false);
    }
    void DropMagazine()
    {
        //GameObject droppedMagazine = Instantiate(magazineHand, magazineHand.transform.position, magazineHand.transform.rotation);
        //droppedMagazine.AddComponent<Rigidbody>();
        //droppedMagazine.AddComponent<BoxCollider>();
        //droppedMagazine.GetComponent<BoxCollider>().size = new Vector3(0.06f, 0.06f, 0.06f);
        magazineHand.SetActive(false);
    }
    void RefillMagazine()
    {
        RaycastWeapon weapon = activeWeapon.GetActiveWeapon();
        weapon.PlaySound("Reload");
        magazineHand.SetActive(true);
    }
    void AttachMagazine()
    {
        RaycastWeapon weapon = activeWeapon.GetActiveWeapon();
        weapon.magazine.SetActive(true);
        magazineHand.SetActive(false); //
        weapon.RefillAmmo();
        //Destroy(magazineHand.gameObject);
        rigController.ResetTrigger("reload_weapon");
        StartCoroutine(WaitForNextAction());
    }

    IEnumerator WaitForNextAction()
    {
        yield return new WaitForSeconds(timeBetNextAction);
        isReloading = false;
    }
}
