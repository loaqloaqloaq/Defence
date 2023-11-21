using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class EnemyBase_Manager : MonoBehaviour
{   
    public GameObject[] enemyBase = new GameObject[3];�@//���݃}�b�v�ɐ�������Ă���EnemyBase
    private Vector3[] movePoint = new Vector3[9];//EnemyBase�̈ړ���
    public GameObject EnemyBase_Prefab;//EnemyBase���󂳂ꂽ��Đ������邽�߂̃v���t�@�u
    public GameObject[] PlayerMovePoint = new GameObject[3];//�ꎞ�I�ɋ����ړ����邽�߁A�v���C���[�̈ړ���
    private GameObject Player;

    public int[] stage = new int[2] { 0, 0 };//���Ă�Q�[�g�̊m�F
    private bool[] moveFlg = new bool[2] { false, false }; //�ړ��������̊m�F
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0;i < movePoint.Length; i++)
        {
            movePoint[i] = GameObject.Find("movePoint" + (i + 1)).transform.position;
        }
        Player = GameObject.Find("Player");
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

    //�Q�[�g����ꂽ�Ƃ��̏���
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
            Player.GetComponent<CharacterController>().enabled = false;
            Player.transform.position = PlayerMovePoint[1].transform.position;
            Player.GetComponent<CharacterController>().enabled = true;
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
            Player.GetComponent<CharacterController>().enabled = false;
            Player.transform.position = PlayerMovePoint[2].transform.position;
            Player.GetComponent<CharacterController>().enabled = true;
        }
    }

    //�Q�[�g���󂵑O�i�������S�Ă̋��_���j�󂳂�Ă��܂����ꍇ
    private void Retreat()
    {
        //��3�X�e�[�W�Ŕj�󂳂ꂽ�ꍇ
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
        //��2�X�e�[�W�ł��ׂĔj�󂳂ꂽ�ꍇ
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
            Debug.Log("����");
            SceneManager.LoadScene("ResultWin");
        }

    }

}
