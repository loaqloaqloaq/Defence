using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;

//-------------------------------------
//仲間(NPC) (今のところ無敵)
//-------------------------------------
public class fellowNPC : MonoBehaviour
{
    //仲間(NPC)の状態 待機、追跡、攻撃
    enum State
    {
        Idle,
        Tracking,
        Attack
    }
    //現在のステート
    [SerializeField] private State fellowAI;

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
    //アニメーター
    [SerializeField] private Animator animator;
    //攻撃可能かの確認
    [SerializeField] private bool attackCheck;
    //武器を使うのに必要なクラス
    [SerializeField] RaycastWeapon weapon;

    void Start()
    {
        //animatorを取得
        animator = GetComponent<Animator>();
        //最初は待機状態
        fellowAI = State.Idle;
        //初期値を設定
        trackingRange = 20.0f;     //追跡範囲
        atkRange = 15.0f;          //攻撃範囲
        enemyDistanceLimit = 1.0f; //敵との距離制限 (近すぎるとき)
        //攻撃力
        attack = 10;                
        weapon.damage = attack;
        //攻撃の確認         
        attackCheck = false;
    }

    void Update()
    {
        //敵の情報を調べる
        enemySearch();
        //Stateの変更
        changeState();

        //仲間(NPC)の行動
        switch (fellowAI)
        {
            //待機状態
            case State.Idle:
                Idle();
                break;
            //追跡状態
            case State.Tracking:
                Tracking();
                break;
            //攻撃状態
            case State.Attack:
                Attack();
                break;
        }
    }

    //待機状態の処理
    private void Idle()
    {
        //攻撃不可能
        attackCheck = false;
        //追跡していない
        animator.SetBool("Tracking", false);
        //攻撃していない
        animator.SetBool("Shooting", false);
    }
    //追跡状態の処理
    private void Tracking()
    {
        //攻撃不可能
        attackCheck = false;
        //攻撃していない
        animator.SetBool("Shooting", false);
        //追跡している
        animator.SetBool("Tracking", true);
        //ターゲット(敵)の座標を取得
        Vector3 targetPos = nearEnemyObj.transform.position;
        //ターゲットのY座標を自分と同じにして2次元に制限
        targetPos.y = this.transform.position.y;
        //ターゲット(敵)の方向へ向かせる
        transform.LookAt(targetPos);
    }
    //攻撃状態の処理
    private void Attack()
    {
        //追跡
        Tracking();
        //攻撃可能にする
        attackCheck = true;
        //銃を撃っている (攻撃している)
        animator.SetBool("Shooting", true);
        //攻撃
        weapon.UpdateNPCWeapon(Time.deltaTime, attackCheck);
    }

    //敵の情報を調べる
    private void enemySearch()
    {
        //自分と1番距離の近い敵を探す
        nearEnemyObj = nearEnemySerch();

        //敵がいない場合
        if (nearEnemyObj == null)
        {
            Debug.Log("敵がフィールド(シーン)にいません");
            return;
        }
        //敵がいた場合
        else if (nearEnemyObj == true)
        {
            //敵と自分の距離を計算
            enemyDistance = Vector3.Distance(nearEnemyObj.transform.position, this.transform.position);
        }
    }

    //Stateの変更
    private void changeState()
    {
        //敵との距離が攻撃範囲内 (15以下)
        if (enemyDistance <= atkRange)
        {
            //攻撃状態にする
            fellowAI = State.Attack;
        }
        //敵との距離が攻撃範囲外 (15以上)
        if (enemyDistance >= atkRange)
        {
            //追跡状態にする
            fellowAI = State.Tracking;
        }
        //敵との距離が追跡範囲外 (20以上)
        if (enemyDistance >= trackingRange)
        {
            //待機状態にする
            fellowAI = State.Idle;
        }
        //敵との距離が追跡範囲外 (1以下)
        else if (enemyDistance <= enemyDistanceLimit)
        {
            //待機状態にする
            fellowAI = State.Idle;
        }
    }

    //Enemyタグの中で最も近いオブジェクトを1つ取得
    private GameObject nearEnemySerch()
    {
        //最も近いオブジェクトの距離を代入するための変数
        float nearDistance = 0;
        //検索された最も近いゲームオブジェクトを代入するための変数
        GameObject searchTargetObj = null;
        //tagNameで指定されたTagを持つ、すべての敵を配列に取得
        GameObject[] enemys = GameObject.FindGameObjectsWithTag(targetLayer);

        //取得した敵が0ならnullを返す(エラーを防ぐため)
        if (enemys.Length == 0)
        {
            return searchTargetObj;
        }
        //すべての敵から１つずつnearEnemy変数に取り出す
        foreach (GameObject nearEnemy in enemys)
        {
            //nearEnemyに取り出した敵と、このNPC(仲間)との距離を計算して取得
            float distance = Vector3.Distance(nearEnemy.transform.position, transform.position);
            //nearDistanceが0(最初はこちら)、もしくはnearDistanceがdistanceよりも大きい値の場合
            if (nearDistance == 0 || nearDistance > distance)
            {
                //nearDistanceを更新
                nearDistance = distance;
                //searchTargetObjを更新
                searchTargetObj = nearEnemy;
            }
        }
        //最も近かったオブジェクトを返す
        return searchTargetObj;
    }
}
