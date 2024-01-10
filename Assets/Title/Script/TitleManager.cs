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
    //�f�t�H���g�̕s�����x
    float defaultOpacity;
    //����m�F��ʂ̕\�����
    bool explanation = false;

    void Start()
    {
        //�f�t�H���g�̕s�����x��ݒ�
        defaultOpacity = 0.8f;
        //�ŏ��͑���m�F��ʂ͔�\��
        explanation = false;
    }

    void Update()
    {
        nowSelectButton = EventSystem.current.currentSelectedGameObject;

        if (nowSelectButton == null)
        {
            if (prevSelectButton == null)
                EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
            else
                EventSystem.current.SetSelectedGameObject(prevSelectButton);

            nowSelectButton = EventSystem.current.currentSelectedGameObject;
        }
        prevSelectButton = EventSystem.current.currentSelectedGameObject;
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
            //�v���O�������I��
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }

    //�I�����Ă���{�^���̃G�t�F�N�g��ς���
    private void ChangeButtonEffect()
    {
        nowSelectButton.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        foreach (Button button in FindObjectsOfType<Button>())
        {
            if (button.gameObject != nowSelectButton)
            {
                Image image = button.GetComponent<Image>();
                if (image != null)
                {
                    image.color = new Color(1, 1, 1, defaultOpacity);
                }
            }
        }
    }

    //�V�[���̓ǂݍ���
    private void LoadScene()
    {
        SceneManager.LoadScene("Test_Map");
    }
    //�Q�[�����J�n
    public void GameStart()
    {
        //0.2�b��ɃV�[����؂�ւ���
        Invoke("LoadScene", 0.2f);

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
