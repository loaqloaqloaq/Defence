using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase_Manager : MonoBehaviour
{   
    public GameObject[] enemyBase = new GameObject[3];　//現在マップに生成されているEnemyBase
    private Vector3[] movePoint = new Vector3[6];//EnemyBaseの移動先
    public GameObject EnemyBase_Prefab;//EnemyBaseが壊されたら再生成するためのプレファブ
    public Vector3[] PlayerMovePoint = new Vector3[3];//一時的に強制移動するため、プレイヤーの移動先

    public int[] stage = new int[2] { 0, 0 };//壊れてるゲートの確認
    private bool[] moveFlg = new bool[2] { false, false }; //移動したかの確認
    // Start is called before the first frame update
    void Start()
    {
        movePoint[0] = new Vector3(-93.41f, 2.45f, 78.4f);
        movePoint[1] = new Vector3(-2.6f, 2.45f, 78.4f);
        movePoint[2] = new Vector3(114.2f, 2.45f, 78.4f);
        movePoint[3] = new Vector3(-88.6f, 2.45f, -170.5f);
        movePoint[4] = new Vector3(10.4f, 2.45f, -170.5f);
        movePoint[5] = new Vector3(119.01f, 2.45f, -170.5f);
    }

    // Update is called once per frame
    void Update()
    {
        Base_Move();
    }

    //ゲートが壊れたときの処理
    private void Base_Move()
    {
        if (stage[0] != 0 && !moveFlg[0])
        {
            if (enemyBase[0] != null) enemyBase[0].gameObject.transform.position = movePoint[0];
            else
            {
                GameObject obj = Instantiate(EnemyBase_Prefab, movePoint[0], transform.rotation);
                enemyBase[0] = obj;
            }
            if (enemyBase[1] != null) enemyBase[1].gameObject.transform.position = movePoint[1];
            else
            {
                GameObject obj = Instantiate(EnemyBase_Prefab, movePoint[1], transform.rotation);
                enemyBase[1] = obj;
            }
            if (enemyBase[2] != null) enemyBase[2].gameObject.transform.position = movePoint[2];
            else
            {
                GameObject obj = Instantiate(EnemyBase_Prefab, movePoint[2], transform.rotation);
                enemyBase[2] = obj;
            }
            moveFlg[0] = true;
        }
        if (stage[1] != 0 && !moveFlg[1])
        {
            if (enemyBase[0] != null) enemyBase[0].gameObject.transform.position = movePoint[3];
            else
            {
                GameObject obj = Instantiate(EnemyBase_Prefab, movePoint[3], transform.rotation);
                enemyBase[0] = obj;
            }
            if (enemyBase[1] != null) enemyBase[1].gameObject.transform.position = movePoint[4];
            else
            {
                GameObject obj = Instantiate(EnemyBase_Prefab, movePoint[4], transform.rotation);
                enemyBase[1] = obj;
            }
            if (enemyBase[2] != null) enemyBase[2].gameObject.transform.position = movePoint[5];
            else
            {
                GameObject obj = Instantiate(EnemyBase_Prefab, movePoint[5], transform.rotation);
                enemyBase[2] = obj;
            }
            moveFlg[1] = true;
        }
    }

}
