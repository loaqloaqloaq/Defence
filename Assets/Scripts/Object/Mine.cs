using UnityEngine;

//-------------------------------------
//地雷 (食らったら一発で死ぬ)
//-------------------------------------
public class Mine : MonoBehaviour
{
    //地雷の攻撃力
    [SerializeField] private int   mine_Damage;
    //ダメージ発生カウント
    private float damage_countDown;
    //消滅する時間
    private float destroy_Time;
    //接触した or しない
    private bool  contact;
    //標的(敵) のLayer
    [SerializeField] private string targetLayer;

    void Start()
    {
        //初期値を設定
        //mine_Damage = 500;       //ダメージ
        damage_countDown = 0.0f; //ダメージ発生カウント
        destroy_Time = 20.0f;    //消滅する時間
        contact = false;         //接触しない
    }

    void Update()
    {
        //接触したら時間をカウント
        if (contact == true)
        {
            damage_countDown += Time.deltaTime;
        }
        //接触してから20秒経ったら
        if (damage_countDown >= destroy_Time)
        {
            //地雷を消滅させる
            Destroy(this.gameObject);
        }
    }

    //地雷に触れた時
    void OnCollisionEnter(Collision collision)
    {
        //インターフェースを取得
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        //当たったlayerを取得
        string hitLayer = LayerMask.LayerToName(collision.gameObject.layer);

        //ダメージ処理が発生したとき
        if (damageable != null)
        {
            //接触した
            contact = true;

            //敵と接触したとき
            if (hitLayer == targetLayer)
            {
                //敵へダメージ処理を行う
                DamageMessage damageMessage = new DamageMessage();
                damageMessage.damager = gameObject;
                damageMessage.amount = mine_Damage;
                damageMessage.hitPoint = collision.transform.position;
                damageMessage.hitNormal = collision.transform.position - transform.position;

                damageable.ApplyDamage(damageMessage);
            }
        }
    }
}
