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
    //クリアタイム
    [SerializeField] private TextMeshProUGUI clearTimeText;
    private float clearTime;

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
        clearTime = PlayerPrefs.GetFloat("timer", 0.0f);

        Debug.Log(" " + clearTime);
        //リザルト画面のTextをセット
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

    //リザルト画面のTextをセット
    private void SetText()
    {
        //1:自分の拠点を守り切ってクリア
        if (Record.resultID == 1)
        {
            //表示するテキストを設定
            resultText.text = "Clear";
            killText.text = "Kill:" + killCount.ToString();
            takenDamageText.text = "Taken Damage:" + takenDamage.ToString();
            scrapText.text = "Used Scrap:" + usedScrapCount.ToString();
        }
        //2:敵拠点を壊し切ってクリア
        else if (Record.resultID == 2)
        {
            //表示するテキストを設定
            resultText.text = "Clear";
            killText.text = "Kill:" + killCount.ToString();
            takenDamageText.text = "Taken Damage:" + takenDamage.ToString();

            //クリアタイム
            float ms = clearTime * 1000;
            string timeStr = String.Format("{0:00}:{1:00}:{2:000}", (int)ms / 60000, (int)(ms / 1000) % 60, ms % 1000);
            clearTimeText.text = "Clear Time:" + timeStr;
        }
        //3:プレイヤの残機がなくなった or 敵拠点を全て壊された
        else if (Record.resultID == 3)
        {
            //表示するテキストを設定
            resultText.text = "Failed";
            killText.text = "Kill:" + killCount.ToString();
        }
    }
}
