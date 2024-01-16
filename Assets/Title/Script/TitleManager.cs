using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//----------------------------------------
//�^�C�g���Ǘ�
//----------------------------------------
public class TitleManager : MonoBehaviour
{
    //�^�C�g���\��UI
    [SerializeField] private GameObject Title_Display;
    //����m�F���
    public GameObject Explanation;
    //���A�I�����Ă���{�^��
    private GameObject nowSelectButton;
    //�O�A�I������Ă����{�^��
    private GameObject prevSelectButton;
    //����m�F��ʂ̕\�����
    bool explanation = false;
    //�C�x���g�V�X�e��
    EventSystem eventSystem;

    void Start()
    {
        //�ŏ��͑���m�F��ʂ͔�\��
        explanation = false;
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
        ChangeButtonEffect();

        //B�L�[����������
        if (Input.GetKeyDown(KeyCode.B) && Explanation.activeSelf)
        {
            //����m�F��ʂ����
            Close_Explanation();
        }
        //"Submit"�{�^�� (Space�L�[)����������
        if (Input.GetButtonDown("Submit") && explanation == false)
        {
            nowSelectButton.GetComponent<Button>().onClick.Invoke();
        }
        //Escape�L�[����������
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Explanation.activeSelf) Close_Explanation();
            else
            {
                //�v���O�������I��
                GameExit();
            }
        }
    }

    //�I�����Ă���{�^���̃G�t�F�N�g��ς���
    private void ChangeButtonEffect()
    {
        foreach (Button button in FindObjectsOfType<Button>())
        {
            if (button.gameObject != nowSelectButton)
            {
                Image image = button.GetComponent<Image>();
            }
        }
    }

    //�V�[���̓ǂݍ���
    private void LoadScene()
    {
        //SceneManager.LoadScene("Test_Map");
        LoadingSceneController.LoadScene("Test_Map");
    }
    //�Q�[�����J�n
    public void GameStart()
    {
        //0.2�b��ɃV�[����؂�ւ���
        //Invoke("LoadScene", 0.2f);
        LoadScene();

        //SoundManager.Instance.Play(enterSE, SoundManager.Sound.UI);
    }

    //����m�F��ʂ��J��
    public void Open_Explanation()
    {
        Title_Display.SetActive(false);
        Explanation.SetActive(true);
        explanation = true;

        //SoundManager.Instance.Play(enterSE, SoundManager.Sound.UI);
    }
    //����m�F��ʂ����
    private void Close_Explanation()
    {
        Title_Display.SetActive(true);
        Explanation.SetActive(false);
        explanation = false;

        //SoundManager.Instance.Play(exitSE, SoundManager.Sound.UI);
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
