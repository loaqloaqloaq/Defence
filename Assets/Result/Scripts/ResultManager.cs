using System;
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
    //���� (�e�L�X�g)
    [SerializeField] private TextMeshProUGUI resultText;
    //�G��|������ (�e�L�X�g)
    [SerializeField] private TextMeshProUGUI killText;
    //�G��|������
    private int killCount;
    //��_���[�W�� (�e�L�X�g)
    [SerializeField] private TextMeshProUGUI takenDamageText;
    //��_���[�W��
    private int takenDamage;
    //������X�N���b�v�̐� (�e�L�X�g)
    [SerializeField] private TextMeshProUGUI scrapText;
    //������X�N���b�v�̐�
    private int usedScrapCount;
    //�N���A�^�C��
    [SerializeField] private TextMeshProUGUI clearTimeText;
    private float clearTime;

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
        //�l���擾
        killCount = PlayerPrefs.GetInt("killCount", 0);
        takenDamage�@= PlayerPrefs.GetInt("playerDamagedCount", 0);
        usedScrapCount = PlayerPrefs.GetInt("usedScrap", 0);
        clearTime = PlayerPrefs.GetFloat("timer", 0.0f);

        Debug.Log(" " + clearTime);
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
        //1:�����̋��_�����؂��ăN���A
        if (Record.resultID == 1)
        {
            //�\������e�L�X�g��ݒ�
            resultText.text = "Clear";
            killText.text = "Kill:" + killCount.ToString();
            takenDamageText.text = "Taken Damage:" + takenDamage.ToString();
            scrapText.text = "Used Scrap:" + usedScrapCount.ToString();
        }
        //2:�G���_���󂵐؂��ăN���A
        else if (Record.resultID == 2)
        {
            //�\������e�L�X�g��ݒ�
            resultText.text = "Clear";
            killText.text = "Kill:" + killCount.ToString();
            takenDamageText.text = "Taken Damage:" + takenDamage.ToString();

            //�N���A�^�C��
            float ms = clearTime * 1000;
            string timeStr = String.Format("{0:00}:{1:00}:{2:000}", (int)ms / 60000, (int)(ms / 1000) % 60, ms % 1000);
            clearTimeText.text = "Clear Time:" + timeStr;
        }
        //3:�v���C���̎c�@���Ȃ��Ȃ��� or �G���_��S�ĉ󂳂ꂽ
        else if (Record.resultID == 3)
        {
            //�\������e�L�X�g��ݒ�
            resultText.text = "Failed";
            killText.text = "Kill:" + killCount.ToString();
        }
    }
}
