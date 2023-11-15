using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager;
using UnityEngine;

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
    [SerializeField]
    private GameObject turret_muzzle;
    //EnemyのTag (一番近くの敵を調べるために使う)
    [SerializeField]
    private string tagName = "Enemy";
    //最も自分に近い敵オブジェクト
    [SerializeField]
    private GameObject nearEnemyObj;
    //敵との距離
    [SerializeField]
    private float enemyDistance;
    //追跡範囲
    [SerializeField] private float trackingDistance;
    //攻撃範囲
    [SerializeField] private float atkRange;
    //敵に最も近い距離
    [SerializeField] private float nearEnemyDistance;
    //RayCastWeaponクラス
    [SerializeField] private RaycastWeapon weapon;
    //敵へ与えるダメージ
    [SerializeField] private int enemyDamage;

    void Start()
    {
        //最初は何もしない
        state = State.Idle;
        //初期値を設定
        trackingDistance = 20.0f;
        atkRange = 15.0f;
        nearEnemyDistance = 2.0f;
        enemyDamage = 1;

        weapon = transform.Find("Gun").GetComponent<RaycastWeapon>();
        weapon.damage = enemyDamage;
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
        if (enemyDistance >= trackingDistance)
        {
            //何もしない
            state = State.Idle;
        }
        else if(enemyDistance <= nearEnemyDistance)
        {
            state = State.Idle;
        }
    }

    //何もしない時の処理
    private void Idle()
    {

    }
    //追跡している時の処理
    private void Tracking()
    {
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
        //攻撃中です
        bool fire = true;
        //追跡
        Tracking();
        //弾を飛ばす処理
        //ダメージ処理
        IDamageable damageable = Serch().gameObject.GetComponent<IDamageable>();
        //nullでないとき
        if (damageable != null)
        {
            damageable.Damage(enemyDamage);
        }
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
        GameObject[] objs = GameObject.FindGameObjectsWithTag(tagName);

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