using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
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
    [SerializeField] private State state;     //現在のステート

    //自身との距離を計算するターゲットオブジェクト (敵)
    [SerializeField]
    private Transform targetObj;
    //敵との距離
    [SerializeField]
    private float enemyDistance;
    [SerializeField] private float trackingDistance = 10.0f; //追跡範囲
    [SerializeField] private float atkRange = 5.0f;          //攻撃範囲
    [SerializeField] private float fieldOfView = 10.0f;      //視野角 

    void Start()
    {
        state = State.Idle;
    }
    void Update()
    {
        enemyDistance = Vector3.Distance(targetObj.transform.position, this.transform.position);

        //Stateの変更
        changeState();
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
        if (enemyDistance <= trackingDistance)
        {
            state = State.Tracking;
        }
        else
        {
            state = State.Idle;
        }
    }
    //何もしないときの処理
    private void Idle()
    {
        this.transform.rotation = Quaternion.Euler(0.0f , 0.0f , 0.0f);
    }

    //追跡している時の処理
    private void Tracking()
    {
        //視野角に敵が入ってきたら一番近くの敵の方向を向く
        if (enemyDistance <= fieldOfView)
        {
            Vector3 targetPos = targetObj.transform.position;
            // ターゲットのY座標を自分と同じにすることで2次元に制限する。
            targetPos.y = this.transform.position.y;
            transform.LookAt(targetPos);
        }

        //※Objectがたくさんいるときの対処法を書く(1体のみ)

    }

    //攻撃している時の処理
    private void Attack()
    {
        //弾を飛ばす処理
        if(enemyDistance <= atkRange)
        {

        }
    }
}
