using UnityEngine;
using UnityEngine.AI;

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

    [SerializeField] Transform respawnPosition;

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

        playerHealth.OnDeath += HandleDeath; //Eventに関数追加
        Cursor.visible = false;

        //UI
        UIManager.Instance.UpdateLifeText(lifeRemains);
    }

    //死ぬときの処理
    private void HandleDeath()
    {
        //プレイやースクリプトオフ
        locomotion.enabled = false;
        aiming.enabled = false;
        activeWeapon.enabled = false;
        reloadWeapon.enabled = false;
        
        //UI
        lifeRemains--;
        UIManager.Instance.UpdateLifeText(lifeRemains);

        //残りライフがゼロになったらゲームオーバ
        if (lifeRemains > 0)
        {
            Invoke("Respawn", 5f); //5秒後に復活
            grController.RefillGrenade();
        }
        else
        {
            GameManager.Instance.EndGame();
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
        gameObject.SetActive(true); //OnEnable呼出
        Cursor.visible = false;
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
            GameManager.Instance.AddItemCount();
            SoundManager.Instance.Play("Sounds/Sfx/PickUp");
        }
    }
}