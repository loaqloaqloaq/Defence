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
    private float spearsMoveSpeed;
    //攻撃のカウント
    private float AttackCount;
    //リセットのカウント
    private float ResetCount;
    //トゲで攻撃をする時間 (5秒)
    [SerializeField] private float spearAttackTime;
    //トゲを元の位置に戻す時間 (5秒)
    [SerializeField] private float spearResetTime;
    //リセットかの確認
    private bool reset = false;

    private void Start()
    {
        //攻撃とリセットの時間
        spearAttackTime = 5.0f;
        spearResetTime = 5.0f;
        //カウントの初期化
        AttackCount = 0.0f;
        ResetCount = 0.0f;
        //リセットではない
        reset = false;
    }
    private void Update()
    {
        //攻撃のカウントを行う
        AttackCount += Time.deltaTime;

        //攻撃カウントが攻撃時間以上かつリセットでないとき
        if (AttackCount >= spearAttackTime && !reset)
        {
            //トゲの攻撃 (トゲが地面から飛び出す)
            SpearsAttack();
        }
        //リセットになったら
        else if(reset == true)
        {
            //リセットのカウントを行う
            ResetCount += Time.deltaTime;
        }

        //カウントがリセット時間以上になったら
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
            //リセットにする
            reset = true;
        }
    }

    //トゲを元の位置に戻す
    private void SpearsReset()
    {
        //0m以下になるまでの間
        if (transform.localPosition.y >= 0.0f)
        {
            //トゲの速度を設定 (降下速度)
            spearsMoveSpeed = -3.0f * Time.deltaTime;
            //トゲの移動
            this.transform.Translate(0, spearsMoveSpeed, 0);
        }
        else
        {
            //カウントの初期化
            AttackCount = 0.0f;
            ResetCount = 0.0f;
            //リセットの初期化
            reset = false;
        }
    }

    //トゲに触れている間
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