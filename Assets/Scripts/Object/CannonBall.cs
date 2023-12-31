using UnityEngine;

//-------------------------------------------------
//砲弾 (プレイヤーの頭上に落下してダメージを与える)
//-------------------------------------------------
public class CannonBall : MonoBehaviour
{
    //標的のLayer (プレイヤー)
    [SerializeField] private string targetLayer;
    //地面のLayer ()
    [SerializeField] private string Floor;
    //プレイヤー (位置取得用)
    [SerializeField] private GameObject player;
    //砲弾の位置情報 (生成されたときの座標)
    private Vector3 cannonBall_Pos;
    //砲弾のY軸方向の移動
    private float ballmoveY;
    //砲弾の攻撃力
    private int cannonBall_Damage;
    //爆発エフェクト
    [SerializeField] private GameObject explosion;
    //砲弾の当たり判定
    [SerializeField] private Collider cannonBall_Collider;
    //砲弾の落下地点マーカー
    [SerializeField] private GameObject marker;
    //マーカー削除用
    //private GameObject mark;
    //落下地点取得用座標
    private Vector3 fallpoint;
    //最初に落ちた瞬間
    private bool firstfall;

    void Start()
    {
        //プレイヤーを取得
        player = GameObject.Find(targetLayer);
        //砲弾を上に上昇させる
        ballmoveY = 1.0f;
        //砲弾の攻撃力を設定
        cannonBall_Damage = 80;
        //マーカー削除用はnull
        //mark = null;
        //砲弾の落下地点の初期設定
        fallpoint = Vector3.zero;
        //落下していない
        firstfall = false;
    }

   void Update()
    {
        //砲弾の行動
        Move();
        //落下ポイントにマーカーを表示
    }

    //砲弾の行動
    private void Move()
    {
        //砲弾の移動
        this.transform.Translate(0, ballmoveY, 0);

        //砲弾が30m以上、上昇したら
        if (this.transform.position.y >= 30.0f && !firstfall)
        {        
            //砲弾の位置を設定 (プレイヤーの20m上)           
            cannonBall_Pos = player.transform.position + new Vector3(0.0f, 29.0f, 0.0f);
            //砲弾の位置を更新
            this.transform.position = cannonBall_Pos;
            //落下地点をプレイヤーの位置に設定
            firstfall = true;

            FallpointMarker();
        }

        if (firstfall)
        {
            //砲弾を下に降下させる
            ballmoveY = -9.8f *Time.deltaTime;
            //当たり判定を有効化する
            cannonBall_Collider.enabled = true;
            //落下開始
            firstfall = true;
        }
    }
    //落下ポイントにマーカーを表示
    public void FallpointMarker()
    {
        fallpoint = player.transform.position;
        //落下ポイントにマーカーを生成
        //mark = Instantiate(marker, fallpoint, Quaternion.identity);
           
        //大きさの設定
        //mark.transform.localScale = new Vector3(2.5f, 0.001f, 2.5f);
        //落下開始時
        if (firstfall == true)
        {

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
        //マーカーを消滅させる
        //Destroy(mark);
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
            DamageMessage damageMessage = new DamageMessage();
            damageMessage.damager = this.gameObject;
            damageMessage.amount = cannonBall_Damage;
            damageMessage.hitPoint = collision.transform.position;
            damageMessage.hitNormal = collision.transform.position - transform.position;

            player.GetComponent<PlayerHealth>().ApplyDamage(damageMessage);
        }
        //爆発を発生させる
        Explotion();
    }
}
