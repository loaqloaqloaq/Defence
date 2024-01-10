using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour //各機能担当するスクリプトを管理するスクリプト
{
    [SerializeField] private int maxLife = 3;
    public int lifeRemains { get; private set; }

    private PlayerLocomotion locomotion; 
    private PlayerAiming aiming; 
    private ActiveWeapon activeWeapon; 
    private ReloadWeapon reloadWeapon;
    private PlayerHealth playerHealth;
    private GrenadeController grController;
    private PlayerTurret playerTurret;
    private PlayerInput input;

    [SerializeField] Transform respawnPosition;

    [SerializeField] private AudioData pickUpSE;

    private const float respawnDelay = 4.0f;

    private void Awake()
    {
        SoundManager.Instance?.AddAudioInfo(pickUpSE);
    }

    //コンポネント取得
    private void Start()
    {
        lifeRemains = maxLife;

        locomotion = GetComponent<PlayerLocomotion>();
        aiming = GetComponent<PlayerAiming>();
        playerHealth = GetComponent<PlayerHealth>();
        activeWeapon = GetComponent<ActiveWeapon>();
        reloadWeapon = GetComponent<ReloadWeapon>();
        grController = GetComponent<GrenadeController>();
        playerTurret = GetComponent<PlayerTurret>();
        input = GetComponent<PlayerInput>();

        playerHealth.OnDeath += HandleDeath; //Eventに関数追加
        Cursor.visible = false;

        //UI
        UIManager.Instance?.UpdateLifeText(lifeRemains);

        //復活位置
        if (!respawnPosition) respawnPosition = transform;
    }

    //死ぬときの処理
    private void HandleDeath()
    {
        //プレイやースクリプトオフ
        locomotion.enabled = false;
        aiming.enabled = false;
        activeWeapon.enabled = false;
        reloadWeapon.enabled = false;
        playerTurret.enabled = false;
        input.enabled = false;
        
        //UI
        lifeRemains--;
        UIManager.Instance?.UpdateLifeText(lifeRemains);
        TurretUI.Instance?.CloseUI();
        //残りライフがゼロになったらゲームオーバ
        if (lifeRemains > 0)
        {
            Invoke("Respawn", respawnDelay); //5秒後に復活
            grController.RefillGrenade();
        }
        else
        {
            Record.resultID = 3;
            GameManager.Instance?.ToResultScene();
        }
    }

    //復活
    public void Respawn()
    {
        //プレイやースクリプトオン
        gameObject.SetActive(false);
        transform.position = respawnPosition.position;
        locomotion.enabled = true;
        aiming.enabled = true;
        activeWeapon.enabled = true;
        reloadWeapon.enabled = true;
        input.enabled = true;
        playerTurret.enabled = true;
        gameObject.SetActive(true); //OnEnable呼出

        UIManager.Instance?.SetMouseVisible(false);
    }

    //アイテムとの衝突処理
    private void OnTriggerEnter(Collider other)
    {
        if (playerHealth.dead)
            return;

        var item = other.GetComponent<IItem>();

        if (item != null)
        {
            item.Use(gameObject);
            //GameManager.Instance.AddItemCount();
            SoundManager.Instance?.PlaySE(pickUpSE.name);
        }
    }
}