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
    //�N���A�^�C�� (�e�L�X�g)
    [SerializeField] private TextMeshProUGUI clearTimeText;
    //�N���A�^�C��
    private float clearTime;
    //�c��^�C�� (�e�L�X�g)
    [SerializeField] private TextMeshProUGUI remainingTimeText;
    //�c��^�C��
    private float remainingTime;
    //YouWin�w�i
    [SerializeField] private GameObject youWin_BG;
    //YouLose�w�i
    [SerializeField] private GameObject youLose_BG;
    //BGM
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip youWin;
    [SerializeField] private AudioClip youLose;
    //���A�I�����Ă���{�^��
    private GameObject nowSelectButton;
    //�O�A�I������Ă����{�^��
    private GameObject prevSelectButton;

    void Awake()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        //�l���擾
        killCount = PlayerPrefs.GetInt("killCount", 0);
        takenDamage�@= PlayerPrefs.GetInt("playerDamagedCount", 0);
        usedScrapCount = PlayerPrefs.GetInt("usedScrap", 0);
        clearTime = PlayerPrefs.GetFloat("clearTime", 0.0f);
        remainingTime = PlayerPrefs.GetFloat("remainingTime", 0.0f);
        //���U���g��ʂŕ\�����镨���Z�b�g
        SetResult();
        //�I�[�f�B�I�̍Đ�
        audioSource.Play();
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

    //�V�[���̓ǂݍ���
    private void LoadScene_Game()
    {
        LoadingSceneController.LoadScene("main");
    }
    //������x�Q�[�����v���C
    public void Retry()
    {
        //SoundManager.Instance.Play(enterSE, SoundManager.Sound.UI);
        //0.2�b��ɃV�[����؂�ւ���
        Invoke("LoadScene_Game", 0.2f);
    }

    //�V�[���̓ǂݍ���
    private void LoadScene_Title()
    {
        SceneManager.LoadScene("Title");
    }
    //�^�C�g���֖߂�
    public void ReturntoTitle()
    {
        //SoundManager.Instance.Play(enterSE, SoundManager.Sound.UI);
        //0.2�b��ɃV�[����؂�ւ���
        Invoke("LoadScene_Title", 0.2f);
    }

    //���U���g��ʂŕ\�����镨���Z�b�g
    private void SetResult()
    {
        //1:���Ԃ��؂��܂Ŏ��w�̋��_�����
        if (Record.resultID == 1)
        {
            //�\������e�L�X�g��ݒ�
            resultText.text = "YouWin!!!!!";
            killText.text = "�L����:" + killCount.ToString();
            takenDamageText.text = "�󂯂��_���[�W��:" + takenDamage.ToString();
            scrapText.text = "�g�����X�N���b�v�̐�:" + usedScrapCount.ToString();
            //YouLose�̎��̔w�i���\��
            youLose_BG.SetActive(false);
            //BGM��ύX
            audioSource.clip = youWin;
        }
        //2:�G�̑O����n��S�Ĕj�󂷂�
        else if (Record.resultID == 2)
        {
            //�\������e�L�X�g��ݒ�
            resultText.text = "YouWin!!!!!";
            killText.text = "�L����:" + killCount.ToString();
            takenDamageText.text = "�󂯂��_���[�W��:" + takenDamage.ToString();
            //�N���A�^�C��
            float ms = clearTime * 1000;
            string timeStr = String.Format("{0:00}:{1:00}:{2:000}", (int)ms / 60000, (int)(ms / 1000) % 60, ms % 1000);
            clearTimeText.text = "�N���A�^�C��:" + timeStr;
            //YouLose�̎��̔w�i���\��
            youLose_BG.SetActive(false);
            //BGM��ύX
            audioSource.clip = youWin;
        }
        //3:�v���C���[�̎c�@���Ȃ��Ȃ� or �ŏI���_���󂳂��
        else if (Record.resultID == 3)
        {
            //�\������e�L�X�g��ݒ�
            resultText.text = "YouLose";
            killText.text = "�L����:" + killCount.ToString();
            //�c��^�C��
            float ms = remainingTime * 1000;
            string timeStr = String.Format("{0:00}:{1:00}:{2:000}", (int)ms / 60000, (int)(ms / 1000) % 60, ms % 1000);
            remainingTimeText.text = "�c��^�C��:" + timeStr;
            //YouWin�̎��̔w�i���\��
            youWin_BG.SetActive(false);
            //BGM��ύX
            audioSource.clip = youLose;
        }
    }
}