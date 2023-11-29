using UnityEngine;

//-------------------------------------------------
//砲弾 (プレイヤーの頭上に落下してくる)_仮
//-------------------------------------------------
public class CannonBall : MonoBehaviour
{
    //標的のLayer (プレイヤー)
    [SerializeField] private string targetLayer;
    //地面のLayer ()
    [SerializeField] private string Floor;
    //プレイヤー (位置取得用)
    [SerializeField] private GameObject player;
    //砲弾の位置
    private Vector3 cannonBall_Pos;
    //砲弾のY軸方向の移動
    private float ballmoveY;
    //砲弾の攻撃力
    private int cannonBall_Damage;
    //爆発エフェクト
    [SerializeField] private GameObject explosion;

    [SerializeField] private Collider cannonBall_Collider;

    void Start()
    {
        //プレイヤーを取得
        player = GameObject.Find(targetLayer);
        //砲弾を上に上昇させる
        ballmoveY = 1.0f;
        //砲弾の攻撃力を設定
        cannonBall_Damage = 80;
    }

   void Update()
    {
        //砲弾の行動
        Move();
    }

    //砲弾の行動
    private void Move()
    {
        //砲弾の移動
        transform.Translate(0, ballmoveY, 0);

        //砲弾が20m以上、上昇したら
        if (transform.position.y >= 20.0f)
        {
            cannonBall_Pos = player.transform.position;
            cannonBall_Pos.y += 20.0f;
            //砲弾の位置を変更
            transform.position = cannonBall_Pos;
            //砲弾を下に降下させる
            ballmoveY = -0.1f;
            //当たり判定を有効化する
            cannonBall_Collider.enabled = true;
        }
    }

    //爆発 (接触)が起きた時の処理
    private void Explotion()
    {
        //爆発エフェクトの位置を設定
        Vector3 expPos = this.transform.position;
        //爆発エフェクト生成
        GameObject exp = Instantiate(explosion, expPos, this.transform.rotation);
        //大きさの設定
        exp.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
        //砲弾を消滅させる
        Destroy(this.gameObject);
    }

    //砲弾に触れた時
    void OnCollisionEnter(Collision collision)
    {
        //当たったlayerを取得
        string hitLayer = LayerMask.LayerToName(collision.gameObject.layer);

        //プレイヤーと接触したとき
        if (hitLayer == targetLayer)
        {
            //プレイヤーへダメージ処理を行う
            DamageMessage damageable = new DamageMessage();
            damageable.damager = this.gameObject;
            damageable.amount = cannonBall_Damage;
            player.GetComponent<PlayerHealth>().ApplyDamage(damageable);
        }
        //爆発を発生させる
        Explotion();
    }
}
