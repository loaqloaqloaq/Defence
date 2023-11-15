using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class teleportController : MonoBehaviour
{
    private GameObject[] PlayerMovePoint = new GameObject[3];//プレイヤーの移動先
    private GameObject Player;
    public GameObject teleportUI;
    public GameObject teleportButton2;//ステージ2に行くことができるボタン
    public GameObject teleportButton3;//ステージ3に行くことができるボタン
    private GameObject[] gate = new GameObject[2];
    public Animator animator;

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
        //yボタンまたはｃキーを押したら
        if(Input.GetButtonDown("Teleport")) {
            if (!isPause)
            {
                //プレイヤーがワープする先を選ぶ
                teleportUI.SetActive(true);
                EventSystem.current.SetSelectedGameObject(GameObject.Find("PlayerteleportUI/stage1"));
                Pause();
            }
            else
            {
                teleportUI.SetActive(false);
                Resume();
            }
        }
        OnTeleportPoint();
    }

    public void SetMouseVisible(bool isVisible)
    {
        Cursor.visible = isVisible;
        Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;
    }
    //一時停止&再開
    public void Pause()
    {
        isPause = true;
        animator.Play("pause_Anim");
        StartCoroutine(SetPanel(true));
        Time.timeScale = 0;
    }
    public void Resume()
    {
        isPause = false;
        animator.Play("resume_Anim");
        StartCoroutine(SetPanel(false));
        Time.timeScale = 1.0f;
    }

    //Pause UI表示
    IEnumerator SetPanel(bool isActive)
    {
        //効果音
        SoundManager.Instance.Play("Sounds/UI_Sfx/Click_Electronic_Pause", SoundManager.Sound.UI);

        while (animator.GetCurrentAnimatorStateInfo(2).normalizedTime < 1.0f)
        {
            yield return new WaitForEndOfFrame();
        }
        SetMouseVisible(isActive);
    }


    //ゲートが壊れていた場合
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

    //任意の場所にテレポート
    public void OnClick_Teleport1Button()
    {
        teleportUI.SetActive(false);
        Player.GetComponent<CharacterController>().enabled = false;
        Player.transform.position = PlayerMovePoint[0].transform.position;
        Player.GetComponent<CharacterController>().enabled = true;
        animator.Play("resume_Anim");
        Time.timeScale = 1;
    }

    public void OnClick_Teleport2Button()
    {
        teleportUI.SetActive(false);
        Player.GetComponent<CharacterController>().enabled = false;
        Player.transform.position = PlayerMovePoint[1].transform.position;
        Player.GetComponent<CharacterController>().enabled = true;
        animator.Play("resume_Anim");
        Time.timeScale = 1;
    }

    public void OnClick_Teleport3Button()
    {
        teleportUI.SetActive(false);
        Player.GetComponent<CharacterController>().enabled = false;
        Player.transform.position = PlayerMovePoint[2].transform.position;
        Player.GetComponent<CharacterController>().enabled = true;
        animator.Play("resume_Anim");
        Time.timeScale = 1;
    }
}
