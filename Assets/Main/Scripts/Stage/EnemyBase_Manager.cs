using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class EnemyBase_Manager : MonoBehaviour
{
    //プレイヤーのワープに関する変数
    [SerializeField]
    bool playerWarp;
    public GameObject counterUI;
    private float teleportCounter;
    public bool teleportFlg;

    public GameObject[] enemyBase = new GameObject[3];　//現在マップに生成されているEnemyBase
    private Vector3[] movePoint = new Vector3[9];//EnemyBaseの移動先
    public GameObject EnemyBase_Prefab;//EnemyBaseが壊されたら再生成するためのプレファブ
    public GameObject[] PlayerMovePoint = new GameObject[3];//一時的に強制移動するため、プレイヤーの移動先
    private GameObject Player;

    public int[] stage = new int[2] { 0, 0 };//壊れてるゲートの確認
    private int[] moveFlg = new int[2] { 0, 0 }; //移動したかの確認
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < movePoint.Length; i++)
        {
            movePoint[i] = GameObject.Find("movePoint" + (i + 1)).transform.position;
        }
        Player = GameObject.Find("Player");
        teleportCounter = 0;
        teleportFlg = false;
        counterUI.SetActive(false);
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
        if (playerWarp && teleportFlg) WarpCounter();
    }

    private void WarpCounter()
    {

        //30秒後にワープ
        if (teleportCounter <= 30.0f)
        {
            counterUI.SetActive(true);
            teleportCounter += Time.deltaTime;
            int Counter = 30 - (int)teleportCounter;
            counterUI.GetComponent<Text>().text = Counter + "秒後に次の拠点へ移動します";
        }
        else
        {
            Debug.Log("ワープ開始");
            PlayerMove();
            EnemyBaseMove();
            teleportCounter = 0;
            teleportFlg = false;
            counterUI.SetActive(false);
        }
    }

    void PlayerMove()
    {
        if (stage[0] != 0 && moveFlg[0] == 0 && teleportFlg)
        {
            Player.GetComponent<CharacterController>().enabled = false;
            Player.transform.position = PlayerMovePoint[1].transform.position;
            Player.GetComponent<CharacterController>().enabled = true;
            moveFlg[0]++;
        }
        if (stage[1] != 0 && moveFlg[1] == 0 && teleportFlg)
        {
            Player.GetComponent<CharacterController>().enabled = false;
            Player.transform.position = PlayerMovePoint[2].transform.position;
            Player.GetComponent<CharacterController>().enabled = true;
            moveFlg[1]++;
        }
    }

    private void EnemyBaseMove()
    {
        if (stage[0] != 0 && moveFlg[0] == 1)
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
            moveFlg[0]++;
        }
        if (stage[1] != 0 && moveFlg[1] == 1)
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
            moveFlg[1]++;
        }

    }

    //ゲートを壊し前進したが全ての拠点が破壊されてしまった場合
    private void Retreat()
    {
        //第3ステージで破壊された場合
        if (stage[1] != 0 && moveFlg[1] == 2)
        {
            GameObject obj = Instantiate(EnemyBase_Prefab, movePoint[3], Quaternion.Euler(0, 180, 0));
            enemyBase[0] = obj;
            obj = Instantiate(EnemyBase_Prefab, movePoint[4], Quaternion.Euler(0, 180, 0));
            enemyBase[1] = obj;
            obj = Instantiate(EnemyBase_Prefab, movePoint[5], Quaternion.Euler(0, 180, 0));
            enemyBase[2] = obj;
            stage[1] = 0;
            moveFlg[1] = 0;
        }
        //第2ステージですべて破壊された場合
        else if (stage[0] != 0 && moveFlg[0] == 2)
        {
            GameObject obj = Instantiate(EnemyBase_Prefab, movePoint[0], Quaternion.Euler(0, 180, 0));
            enemyBase[0] = obj;
            obj = Instantiate(EnemyBase_Prefab, movePoint[1], Quaternion.Euler(0, 180, 0));
            enemyBase[1] = obj;
            obj = Instantiate(EnemyBase_Prefab, movePoint[2], Quaternion.Euler(0, 180, 0));
            enemyBase[2] = obj;
            stage[0] = 0;
            moveFlg[0] = 0;
        }
        else
        {
            Debug.Log("勝利");
            SceneManager.LoadScene("ResultWin");
        }

    }

}
