using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase_Manager : MonoBehaviour
{   
    public GameObject[] enemyBase = new GameObject[3];　//現在マップに生成されているEnemyBase
    private Vector3[] movePoint = new Vector3[9];//EnemyBaseの移動先
    private Vector3 rotation;
    public GameObject EnemyBase_Prefab;//EnemyBaseが壊されたら再生成するためのプレファブ
    public GameObject[] PlayerMovePoint = new GameObject[3];//一時的に強制移動するため、プレイヤーの移動先
    public GameObject Player;

    public int[] stage = new int[2] { 0, 0 };//壊れてるゲートの確認
    private bool[] moveFlg = new bool[2] { false, false }; //移動したかの確認
    // Start is called before the first frame update
    void Start()
    {
        movePoint[0] = new Vector3(-93.92f, 27.11f, 290.61f);
        movePoint[1] = new Vector3(12.2f, 27.1f, 295.67f);
        movePoint[2] = new Vector3(115.33f, 23.92f, 292.09f);
        movePoint[3] = new Vector3(-93.41f, 2.45f, 78.4f);
        movePoint[4] = new Vector3(-2.6f, 2.45f, 78.4f);
        movePoint[5] = new Vector3(114.2f, 2.45f, 78.4f);
        movePoint[6] = new Vector3(-88.6f, 2.45f, -170.5f);
        movePoint[7] = new Vector3(10.4f, 2.45f, -170.5f);
        movePoint[8] = new Vector3(119.01f, 2.45f, -170.5f);
        rotation = new Vector3(0, 180, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        if (enemyBase[0] == null && enemyBase[1] == null && enemyBase[2] == null)
        {
            Retreat();
        }
    }

    //ゲートが壊れたときの処理
    private void Move()
    {
        if (stage[0] != 0 && !moveFlg[0])
        {
            if (enemyBase[0] != null) enemyBase[0].gameObject.transform.position = movePoint[3];
            else
            {
                GameObject obj = Instantiate(EnemyBase_Prefab, movePoint[3], Quaternion.Euler(0, 180, 0));
                enemyBase[0] = obj;
            }
            if (enemyBase[1] != null) enemyBase[1].gameObject.transform.position = movePoint[4];
            else
            {
                GameObject obj = Instantiate(EnemyBase_Prefab, movePoint[4], Quaternion.Euler(0, 180, 0));
                enemyBase[1] = obj;
            }
            if (enemyBase[2] != null) enemyBase[2].gameObject.transform.position = movePoint[5];
            else
            {
                GameObject obj = Instantiate(EnemyBase_Prefab, movePoint[5], Quaternion.Euler(0, 180, 0));
                enemyBase[2] = obj;
            }
            moveFlg[0] = true;
            Player.transform.position = PlayerMovePoint[1].transform.position;
        }
        if (stage[1] != 0 && !moveFlg[1])
        {
            if (enemyBase[0] != null) enemyBase[0].gameObject.transform.position = movePoint[6];
            else
            {
                GameObject obj = Instantiate(EnemyBase_Prefab, movePoint[6], Quaternion.Euler(0, 180, 0));
                enemyBase[0] = obj;
            }
            if (enemyBase[1] != null) enemyBase[1].gameObject.transform.position = movePoint[7];
            else
            {
                GameObject obj = Instantiate(EnemyBase_Prefab, movePoint[7], Quaternion.Euler(0, 180, 0));
                enemyBase[1] = obj;
            }
            if (enemyBase[2] != null) enemyBase[2].gameObject.transform.position = movePoint[8];
            else
            {
                GameObject obj = Instantiate(EnemyBase_Prefab, movePoint[8], Quaternion.Euler(0, 180, 0));
                enemyBase[2] = obj;
            }
            moveFlg[1] = true;
            Player.transform.position = PlayerMovePoint[2].transform.position;
        }
    }

    //ゲートを壊し前進したが全ての拠点が破壊されてしまった場合
    private void Retreat()
    {
        //第3ステージで破壊された場合
        if (stage[1] != 0 && moveFlg[1])
        {
            GameObject obj = Instantiate(EnemyBase_Prefab, movePoint[3], Quaternion.Euler(0, 180, 0));
            enemyBase[0] = obj;
            obj = Instantiate(EnemyBase_Prefab, movePoint[4], Quaternion.Euler(0, 180, 0));
            enemyBase[1] = obj;
            obj = Instantiate(EnemyBase_Prefab, movePoint[5], Quaternion.Euler(0, 180, 0));
            enemyBase[2] = obj;
            stage[1] = 0;
            moveFlg[1] = false;
        }
        //第2ステージですべて破壊された場合
        else if (stage[0] != 0 && moveFlg[0])
        {
            GameObject obj = Instantiate(EnemyBase_Prefab, movePoint[0], Quaternion.Euler(0, 180, 0));
            enemyBase[0] = obj;
            obj = Instantiate(EnemyBase_Prefab, movePoint[1], Quaternion.Euler(0, 180, 0));
            enemyBase[1] = obj;
            obj = Instantiate(EnemyBase_Prefab, movePoint[2], Quaternion.Euler(0, 180, 0));
            enemyBase[2] = obj;
            stage[0] = 0;
            moveFlg[0] = false;
        }
        else
        {
            Debug.Log("勝利");
        }
        
    }

}
