using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//-----------------------------------------
//リザルト管理
//-----------------------------------------
public class ResultManager : MonoBehaviour
{
    //結果 (テキスト)
    [SerializeField] private TextMeshProUGUI resultText;
    //敵を倒した数 (テキスト)
    [SerializeField] private TextMeshProUGUI killText;
    //敵を倒した数
    private int killCount;
    //被ダメージ量 (テキスト)
    [SerializeField] private TextMeshProUGUI takenDamageText;
    //被ダメージ量
    private int takenDamage;
    //消費したスクラップの数 (テキスト)
    [SerializeField] private TextMeshProUGUI scrapText;
    //消費したスクラップの数
    private int usedScrapCount;
    //クリアタイム (テキスト)
    [SerializeField] private TextMeshProUGUI clearTimeText;
    //クリアタイム
    private float clearTime;
    //残りタイム (テキスト)
    [SerializeField] private TextMeshProUGUI remainingTimeText;
    //残りタイム
    private float remainingTime;
    //YouWin背景
    [SerializeField] private GameObject youWin_BG;
    //YouLose背景
    [SerializeField] private GameObject youLose_BG;
    //BGM
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip youWin;
    [SerializeField] private AudioClip youLose;
    //今、選択しているボタン
    private GameObject nowSelectButton;
    //前、選択されていたボタン
    private GameObject prevSelectButton;
    //デフォルトの不透明度
    float defaultOpacity;

    void Start()
    {
        //デフォルトの不透明度を設定
        defaultOpacity = 0.8f;
        //値を取得
        killCount = PlayerPrefs.GetInt("killCount", 0);
        takenDamage　= PlayerPrefs.GetInt("playerDamagedCount", 0);
        usedScrapCount = PlayerPrefs.GetInt("usedScrap", 0);
        clearTime = PlayerPrefs.GetFloat("clearTime", 0.0f);
        remainingTime = PlayerPrefs.GetFloat("remainingTime", 0.0f);
        //リザルト画面で表示する物をセット
        SetResult();
        //オーディオの再生
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
        ChangeButtonEffect();

        //"Submit"ボタン (Spaceキー)を押したら
        if (Input.GetButtonDown("Submit"))
        {
            nowSelectButton.GetComponent<Button>().onClick.Invoke();
        }
        //Escapeキーを押したら
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //プログラムを終了
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }

    //選択しているボタンのエフェクトを変える
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

    //シーンの読み込み
    private void LoadScene_Game()
    {
        SceneManager.LoadScene("Test_Map");
    }
    //もう一度ゲームをプレイ
    public void Retry()
    {
        //0.2秒後にシーンを切り替える
        Invoke("LoadScene_Game", 0.2f);

        //SoundManager.Instance.Play(enterSE, SoundManager.Sound.UI);
    }

    //シーンの読み込み
    private void LoadScene_Title()
    {
        SceneManager.LoadScene("Title");
    }
    //タイトルへ戻る
    public void ReturntoTitle()
    {
        //0.2秒後にシーンを切り替える
        Invoke("LoadScene_Title", 0.2f);

        //SoundManager.Instance.Play(enterSE, SoundManager.Sound.UI);
    }

    //リザルト画面で表示する物をセット
    private void SetResult()
    {
        //1:時間が切れるまで自陣の拠点を守る
        if (Record.resultID == 1)
        {
            //表示するテキストを設定
            resultText.text = "YouWin!!!!!";
            killText.text = "キル数:" + killCount.ToString();
            takenDamageText.text = "受けたダメージ量:" + takenDamage.ToString();
            scrapText.text = "使ったスクラップの数:" + usedScrapCount.ToString();
            //YouWinの時の背景を表示
            youWin_BG.SetActive(true);
            //BGMを変更
            audioSource.clip = youWin;
        }
        //2:敵の前線基地を全て破壊する
        else if (Record.resultID == 2)
        {
            //表示するテキストを設定
            resultText.text = "YouWin!!!!!";
            killText.text = "キル数:" + killCount.ToString();
            takenDamageText.text = "受けたダメージ量:" + takenDamage.ToString();
            //クリアタイム
            float ms = clearTime * 1000;
            string timeStr = String.Format("{0:00}:{1:00}:{2:000}", (int)ms / 60000, (int)(ms / 1000) % 60, ms % 1000);
            clearTimeText.text = "クリアタイム:" + timeStr;
            //YouWinの時の背景を表示
            youWin_BG.SetActive(true);
            //BGMを変更
            audioSource.clip = youWin;
        }
        //3:プレイヤーの残機がなくなる or 最終拠点が壊される
        else if (Record.resultID == 3)
        {
            //表示するテキストを設定
            resultText.text = "YouLose";
            killText.text = "キル数:" + killCount.ToString();
            //残りタイム
            float ms = remainingTime * 1000;
            string timeStr = String.Format("{0:00}:{1:00}:{2:000}", (int)ms / 60000, (int)(ms / 1000) % 60, ms % 1000);
            remainingTimeText.text = "残りタイム:" + timeStr;
            //YouLoseの時の背景を表示
            youLose_BG.SetActive(true);
            //BGMを変更
            audioSource.clip = youLose;
        }
    }
}