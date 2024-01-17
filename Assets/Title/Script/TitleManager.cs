using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//----------------------------------------
//�^�C�g���Ǘ�
//----------------------------------------
public class TitleManager : MonoBehaviour
{
    //�^�C�g���\��UI
    [SerializeField] private GameObject Title_Display;
    //���A�I�����Ă���{�^��
    private GameObject nowSelectButton;
    //�O�A�I������Ă����{�^��
    private GameObject prevSelectButton;
    //�C�x���g�V�X�e��
    EventSystem eventSystem;

    void Start()
    {
        Time.timeScale = 1f;

        eventSystem = EventSystem.current;

        nowSelectButton = null;
        prevSelectButton = null;
    }

    void Update()
    {
        nowSelectButton = eventSystem.currentSelectedGameObject;

        if (nowSelectButton == null)
        {
            if (prevSelectButton == null)
                eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);
            else
                eventSystem.SetSelectedGameObject(prevSelectButton);

            nowSelectButton = eventSystem.currentSelectedGameObject;
        }
        prevSelectButton = eventSystem.currentSelectedGameObject;

        //"Submit"�{�^�� (Space�L�[)����������
        if (Input.GetButtonDown("Submit"))
        {
            nowSelectButton.GetComponent<Button>().onClick.Invoke();
        }
        //Escape�L�[����������
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //�v���O�������I��
            GameExit();
        }
    }

    //�V�[���̓ǂݍ���
    private void LoadScene()
    {
        LoadingSceneController.LoadScene("Test_Map");
    }

    //�Q�[�����J�n
    public void GameStart()
    {
        //SoundManager.Instance.Play(enterSE, SoundManager.Sound.UI);
        //�V�[���̓ǂݍ���
        LoadScene();
    }

    //�Q�[�����I��
    public void GameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        //SoundManager.Instance.Play(exitSE, SoundManager.Sound.UI);
        Application.Quit();
    }
}
