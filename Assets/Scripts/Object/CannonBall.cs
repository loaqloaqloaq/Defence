using UnityEngine;
using UnityEngine.UI;

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
    //プレイヤーの入力 (マーカーの位置調整用)
    //[SerializeField] PlayerInput playerinput;
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
    private GameObject mark;
    //落下地点取得用座標
    private Vector3 fallpoint;
    //最初に落ちた瞬間
    private bool firstfall;

    void Start()
    {
        //プレイヤーを取得
        player = GameObject.Find(targetLayer);
        //プレイヤーの入力を取得
        //playerinput = player.GetComponent<PlayerInput>();   
        //砲弾を上に上昇させる
        ballmoveY = 1.0f;
        //砲弾の攻撃力を設定
        cannonBall_Damage = 80;
        //マーカー削除用はnull
        mark = null;
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
        FallpointMarker();
    }

    //砲弾の行動
    private void Move()
    {
        //砲弾の移動
        this.transform.Translate(0, ballmoveY, 0);

        //砲弾が30m以上、上昇したら
        if (transform.position.y >= 30.0f)
        {
            //砲弾の位置を設定
            cannonBall_Pos = player.transform.position;
            cannonBall_Pos.y += 20.0f;
            //砲弾の位置を更新
            this.transform.position = cannonBall_Pos;
            //砲弾を下に降下させる
            ballmoveY = -0.1f;
            //当たり判定を有効化する
            cannonBall_Collider.enabled = true;
            //落下開始
            firstfall = true;
        }
    }
    //落下ポイントにマーカーを表示
    private void FallpointMarker()
    {
        //落下開始時
        if (firstfall == true)
        {
            //落下地点をプレイヤーの位置にする
            fallpoint = player.transform.position;
            //落下ポイントにマーカーを生成
            mark = Instantiate(marker, fallpoint, Quaternion.identity);
            //大きさの設定
            mark.transform.localScale = new Vector3(2.5f, 0.001f, 2.5f);
            //既に落下している
            firstfall = false;
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
        Destroy(mark);
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
