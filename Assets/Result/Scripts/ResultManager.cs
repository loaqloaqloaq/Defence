using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//-----------------------------------------
//���U���g�Ǘ�
//-----------------------------------------
public class ResultManager : MonoBehaviour
{
    //���� (YouWin or YouLose)
    [SerializeField] private TextMeshProUGUI resultText;

    //�G��|������ (��)
    //[SerializeField] private TextMeshProUGUI killText;
    //�N���A�^�C�� (��)
    //[SerializeField] private TextMeshProUGUI clearTimeText;

    //���A�I�����Ă���{�^��
    private GameObject nowSelectButton;
    //�O�A�I������Ă����{�^��
    private GameObject prevSelectButton;
    //�f�t�H���g�̕s�����x
    float defaultOpacity;
    void Start()
    {
        //�f�t�H���g�̕s�����x��ݒ�
        defaultOpacity = 0.8f;
        //���U���g��ʂ�Text���Z�b�g
        SetText();
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

        //"Submit"�{�^�� (Space�L�[)����������
        if (Input.GetButtonDown("Submit"))
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
    private void LoadScene_Game()
    {
        SceneManager.LoadScene("Test_Map");
    }
    //������x�Q�[�����v���C
    public void Retry()
    {
        //0.2�b��ɃV�[����؂�ւ���
        Invoke("LoadScene_Game", 0.2f);

        //SoundManager.Instance.Play(enterSE, SoundManager.Sound.UI);
    }

    //�V�[���̓ǂݍ���
    private void LoadScene_Title()
    {
        SceneManager.LoadScene("Title");
    }
    //�^�C�g���֖߂�
    public void ReturntoTitle()
    {
        //0.2�b��ɃV�[����؂�ւ���
        Invoke("LoadScene_Title", 0.2f);

        //SoundManager.Instance.Play(enterSE, SoundManager.Sound.UI);
    }

    //���U���g��ʂ�Text���Z�b�g
    private void SetText()
    {
        //���ݒ�
        if (Record.result == "Failed")
        { resultText.text = "Failed"; }
        else
        { resultText.text = "Clear"; }
    }
}
