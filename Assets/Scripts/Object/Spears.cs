using UnityEngine;

//-------------------------------------------
//トゲのトラップ (通るとダメージを与えるゾーン)
//-------------------------------------------
public class Spears : MonoBehaviour
{
    //標的(敵) のLayer
    [SerializeField] private string targetLayer = "Enemy";
    //トゲの攻撃力
    [SerializeField] private int spearsDamage;
    //トゲの移動速度
    [SerializeField] private float spearsMoveSpeed;
    //トゲの位置
    [SerializeField] private Vector2 spearsPos;
    //攻撃のカウント
    [SerializeField] private float AttackCount;
    //リセットのカウント
    [SerializeField] private float ResetCount;
    //トゲで攻撃をする時間 (5秒)
    [SerializeField] private float spearAttackTime;
    //トゲを元に戻す時間 (10秒)
    [SerializeField] private float spearResetTime;

    private void Start()
    {
        //トゲの初期位置
        spearsPos = this.transform.position;
        //攻撃とリセットの時間
        spearAttackTime = 5.0f;
        spearResetTime = 10.0f;
        //カウントの初期化
        AttackCount = 0.0f;
        ResetCount = 0.0f;
    }
    private void Update()
    {
        //攻撃のカウントを行う
        AttackCount += Time.deltaTime;

        //カウントが攻撃時間になったら
        if (AttackCount >= spearAttackTime)
        {
            //トゲの攻撃 (トゲが地面から飛び出す)
            SpearsAttack();
        }
        //カウントがリセット時間になったら
        if (ResetCount >= spearResetTime)
        {
            //トゲを元の位置に戻す
            SpearsReset();
        }
    }

    //トゲの攻撃 (トゲが地面から飛び出す)
    private void SpearsAttack()
    {
        //1.5m以上上昇するまでの間
        if (transform.localPosition.y <= 1.5f)
        {
            //トゲの速度を設定 (上昇速度)
            spearsMoveSpeed = 3.0f * Time.deltaTime;
            //トゲの移動
            this.transform.Translate(0, spearsMoveSpeed, 0);
        }
        else
        {
            //リセットのカウントを行う
            ResetCount += Time.deltaTime;
        }
    }

    //トゲを元の位置に戻す
    private void SpearsReset()
    {
        //0m以上の時
        if (transform.localPosition.y >= 0.0f)
        {
            //トゲの位置を完全にリセット
            this.transform.position = spearsPos;
            //攻撃カウントをもとに戻す
            AttackCount = 0.0f;
            //リセットカウントをもとに戻す
            ResetCount = 0.0f;
        }
    }

    //地雷に触れている間
    void OnCollisionStay(Collision collision)
    {
        //当たったオブジェクトのインターフェースを取得
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        //当たったオブジェクトのlayerを取得
        string hitLayer = LayerMask.LayerToName(collision.gameObject.layer);

        //敵と接触したとき
        if (hitLayer == targetLayer)
        {
            Debug.Log("敵に触れている");

            DamageMessage damageMessage = new DamageMessage();
            damageMessage.damager = gameObject;
            damageMessage.amount = spearsDamage;
            damageMessage.hitPoint = collision.transform.position;
            damageMessage.hitNormal = collision.transform.position - transform.position;
            //敵へダメージ処理を行う
            damageable.ApplyDamage(damageMessage);
        }
    }
}