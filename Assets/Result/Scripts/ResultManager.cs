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
    //結果 (YouWin or YouLose)
    [SerializeField] private TextMeshProUGUI resultText;

    //敵を倒した数 (仮)
    //[SerializeField] private TextMeshProUGUI killText;
    //クリアタイム (仮)
    //[SerializeField] private TextMeshProUGUI clearTimeText;

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
        //仮設定
        if (Record.result == "Failed")
        { resultText.text = "Failed"; }
        else
        { resultText.text = "Clear"; }
    }
}
