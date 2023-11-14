using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static RaycastWeapon;

public class turret : RaycastWeapon
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
    //敵に近すぎる距離
    [SerializeField] private float nearEnemyDistance;


    void Start()
    {
        //最初は何もしない
        state = State.Idle;
        //初期値を設定
        trackingDistance = 10.0f;
        atkRange = 5.0f;
        nearEnemyDistance = 2.0f;
    }
    void Update()
    {

        //敵がいるか調べる
        nearEnemyObj = Serch();

        //敵がいないとき
        if (nearEnemyObj == null)
        {
            Debug.Log("敵がフィールド上にいません");
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
        //敵との距離が攻撃範囲内 (5以下)
        if (enemyDistance <= atkRange)
        {
            //攻撃する
            state = State.Attack;
        }
        //敵との距離が攻撃範囲外 (5以上)
        if (enemyDistance >= atkRange)
        {
            //追跡する
            state = State.Tracking;
        }
        //敵との距離が追跡範囲外
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

    //何もしないときの処理
    private void Idle()
    {
        //Debug.Log("何もしません");
        this.transform.rotation = Quaternion.Euler(0.0f , 0.0f , 0.0f);
    }
    //追跡している時の処理
    private void Tracking()
    {
        //Debug.Log("追跡します");
        //ターゲット(敵)の座標を取得
        Vector3 targetPos = nearEnemyObj.transform.position;
        //ターゲットのY座標を自分と同じにすることで2次元に制限する。
        targetPos.y = this.transform.position.y;
        //タレットを敵へ向ける
        transform.LookAt(targetPos);
    }
    //攻撃している時の処理
    private void Attack()
    {
        Debug.Log("攻撃します");
        //追跡
        Tracking();
        //弾を飛ばす処理
        //gameObject = Shot;
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