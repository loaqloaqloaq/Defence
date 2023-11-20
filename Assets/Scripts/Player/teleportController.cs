using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class teleportController : MonoBehaviour
{
    private GameObject[] PlayerMovePoint = new GameObject[3];//�v���C���[�̈ړ���
    private GameObject Player;
    public GameObject teleportUI;
    public GameObject teleportButton2;//�X�e�[�W2�ɍs�����Ƃ��ł���{�^��
    public GameObject teleportButton3;//�X�e�[�W3�ɍs�����Ƃ��ł���{�^��
    private GameObject[] gate = new GameObject[2];

    public bool isPause { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("Player");
        PlayerMovePoint[0] = GameObject.Find("teleportPoint1");
        PlayerMovePoint[1] = GameObject.Find("teleportPoint2");
        PlayerMovePoint[2] = GameObject.Find("teleportPoint3");
        teleportUI.SetActive(false);
        gate[0] = GameObject.Find("Gate1");
        gate[1] = GameObject.Find("Gate2");
        isPause = false;
    }

    // Update is called once per frame
    void Update()
    {
        //y�{�^���܂��͂��L�[����������
        if(Input.GetButtonDown("Teleport")) {
            if (!isPause)
            {
                //�v���C���[�����[�v������I��
                teleportUI.SetActive(true);
                EventSystem.current.SetSelectedGameObject(GameObject.Find("PlayerteleportUI/stage1"));
                Time.timeScale = 0;
                isPause = true;
            }
            else
            {
                teleportUI.SetActive(false);
                isPause = false;
                Time.timeScale = 1;
            }
        }
        OnTeleportPoint();
    }

    public void SetMouseVisible(bool isVisible)
    {
        Cursor.visible = isVisible;
        Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;
    }

    //�Q�[�g�����Ă����ꍇ
    void OnTeleportPoint()
    {
        if (gate[0].GetComponent<GateController>().HP <= 0) {
            teleportButton2.SetActive(true);
        }
        else
        {
            teleportButton2.SetActive(false);
        }
        if (gate[1].GetComponent<GateController>().HP <= 0)
        {
            teleportButton3.SetActive(true);
        }
        else
        {
            teleportButton3.SetActive(false);
        }
    }

    //�C�ӂ̏ꏊ�Ƀe���|�[�g
    public void OnClick_Teleport1Button()
    {
        teleportUI.SetActive(false);
        Player.GetComponent<CharacterController>().enabled = false;
        Player.transform.position = PlayerMovePoint[0].transform.position;
        Player.GetComponent<CharacterController>().enabled = true;
        Time.timeScale = 1;
    }

    public void OnClick_Teleport2Button()
    {
        teleportUI.SetActive(false);
        Player.GetComponent<CharacterController>().enabled = false;
        Player.transform.position = PlayerMovePoint[1].transform.position;
        Player.GetComponent<CharacterController>().enabled = true;
        Time.timeScale = 1;
    }

    public void OnClick_Teleport3Button()
    {
        teleportUI.SetActive(false);
        Player.GetComponent<CharacterController>().enabled = false;
        Player.transform.position = PlayerMovePoint[2].transform.position;
        Player.GetComponent<CharacterController>().enabled = true;
        Time.timeScale = 1;
    }
}
