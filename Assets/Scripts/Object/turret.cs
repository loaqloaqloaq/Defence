using UnityEngine;

//-------------------------------------------------
//タレット (敵を自動で攻撃する)
//-------------------------------------------------
public class turret : MonoBehaviour
{
    //タレットの状態 何もしない,追跡,攻撃
    private enum State
    {
        Idle,
        Tracking,
        Attack
    }
    //現在のステート
    [SerializeField] private State state;

    //タレットの銃口
    [SerializeField] private GameObject turret_muzzle;
    //標的のLayer (一番近くの敵を調べるために使う)
    [SerializeField] private string targetLayer;
    //敵との距離
    [SerializeField] private float enemyDistance;
    //追跡範囲
    [SerializeField] private float trackingRange;
    //攻撃範囲
    [SerializeField] private float atkRange;
    //敵との距離制限 (近すぎるとき)
    [SerializeField] private float enemyDistanceLimit;
    //最も自分に近い敵オブジェクト
    [SerializeField] private GameObject nearEnemyObj;
    //敵へ与えるダメージ
    [SerializeField] private int attack;
    //攻撃可能かの確認
    [SerializeField] private bool attackCheck;
    //武器を使うのに必要なクラス
    [SerializeField] private RaycastWeapon weapon;

    void Start()
    {
        //最初は待機状態
        state = State.Idle;
        //初期値を設定
        trackingRange = 20.0f;     //追跡範囲
        atkRange = 15.0f;          //攻撃範囲
        enemyDistanceLimit = 2.0f; //敵との距離制限 (近すぎるとき)
        //攻撃力
        attack = 1;
        weapon.damage = attack;
        //攻撃の確認         
        attackCheck = false;
    }
    void Update()
    {
        //敵がいるか調べる
        nearEnemyObj = Serch();

        //敵がいないとき
        if (nearEnemyObj == null)
        {
            //Debug.Log("敵がフィールド上にいません");
            return;
        }
        //敵がいた場合
        else if (nearEnemyObj == true)
        {
            //敵との距離を計算
            enemyDistance = Vector3.Distance(nearEnemyObj.transform.position, this.transform.position);
        }

        //Stateの変更
        changeState();

        //State毎の行動
        switch (state)
        {
            case State.Idle:
                Idle();
                break;
            case State.Tracking:
                Tracking();
                break;
            case State.Attack:
                Attack();
                break;
        }
    }

    //Stateの変更
    private void changeState()
    {
        //敵との距離が攻撃範囲内 (15以下)
        if (enemyDistance <= atkRange)
        {
            //攻撃する
            state = State.Attack;
        }
        //敵との距離が攻撃範囲外 (15以上)
        if (enemyDistance >= atkRange)
        {
            //追跡する
            state = State.Tracking;
        }
        //敵との距離が追跡範囲外 (20以上)
        if (enemyDistance >= trackingRange)
        {
            //何もしない
            state = State.Idle;
        }
        else if(enemyDistance <= enemyDistanceLimit)
        {
            state = State.Idle;
        }
    }

    //何もしない時の処理
    private void Idle()
    {
        //攻撃不可能
        attackCheck = false;
    }
    //追跡している時の処理
    private void Tracking()
    {
        //攻撃不可能
        attackCheck = false;
        //ターゲット(敵)の座標を取得
        Vector3 targetPos = nearEnemyObj.transform.position;
        //ターゲットのY座標を自分と同じにすることで2次元に制限する。
        targetPos.y = this.transform.position.y;
        //タレットを敵へ向ける
        transform.LookAt(targetPos);
        //銃口の向く方向を指定
        turret_muzzle.transform.LookAt(Serch().transform);
    }
    //攻撃している時の処理
    private void Attack()
    {
        //追跡
        Tracking();
        //攻撃可能にする
        attackCheck = true;
        //弾を飛ばす処理
        weapon.UpdateNPCWeapon(Time.deltaTime, attackCheck);
    }

    //※Objectがたくさんいるときの対処法 (1体のみ)
    //Enemyタグの中で最も近いものを取得
    private GameObject Serch()
    {
        // 最も近いオブジェクトの距離を代入するための変数
        float nearDistance = 0;
        // 検索された最も近いゲームオブジェクトを代入するための変数
        GameObject searchTargetObj = null;
        // tagNameで指定されたTagを持つ、すべてのゲームオブジェクトを配列に取得
        GameObject[] objs = GameObject.FindGameObjectsWithTag(targetLayer);

        // 取得したゲームオブジェクトが 0 ならnullを戻す(使用する場合にはnullでもエラーにならない処理にしておく)
        if (objs.Length == 0)
        {
            return searchTargetObj;
        }

        // objsから１つずつobj変数に取り出す
        foreach (GameObject obj in objs)
        {
            // objに取り出したゲームオブジェクトと、このゲームオブジェクトとの距離を計算して取得
            float distance = Vector3.Distance(obj.transform.position, transform.position);
            // nearDistanceが0(最初はこちら)、あるいはnearDistanceがdistanceよりも大きい値なら
            if (nearDistance == 0 || nearDistance > distance)
            {
                // nearDistanceを更新
                nearDistance = distance;
                // searchTargetObjを更新
                searchTargetObj = obj;
            }
        }
        //最も近かったオブジェクトを返す
        return searchTargetObj;
    }
}