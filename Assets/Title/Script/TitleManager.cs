using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//----------------------------------------
//タイトル管理
//----------------------------------------
public class TitleManager : MonoBehaviour
{
    //タイトル表示UI
    [SerializeField] private GameObject Title_Display;
    //操作確認画面
    public GameObject Explanation;
    //今、選択しているボタン
    private GameObject nowSelectButton;
    //前、選択されていたボタン
    private GameObject prevSelectButton;
    //デフォルトの不透明度
    float defaultOpacity;
    //操作確認画面の表示状態
    bool explanation = false;

    void Start()
    {
        //デフォルトの不透明度を設定
        defaultOpacity = 0.8f;
        //最初は操作確認画面は非表示
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

        //Bキーを押したら
        if (Input.GetKeyDown(KeyCode.B) && Explanation.activeSelf)
        {
            //操作確認画面を閉じる
            Close_Explanation();
        }
        //"Submit"ボタン (Spaceキー)を押したら
        if (Input.GetButtonDown("Submit") && explanation == false)
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
    private void LoadScene()
    {
        SceneManager.LoadScene("Test_Map");
    }
    //ゲームを開始
    public void GameStart()
    {
        //0.2秒後にシーンを切り替える
        Invoke("LoadScene", 0.2f);

        //SoundManager.Instance.Play(enterSE, SoundManager.Sound.UI);
    }

    //操作確認画面を開く
    public void Open_Explanation()
    {
        Title_Display.SetActive(false);
        Explanation.SetActive(true);
        explanation = true;

        //SoundManager.Instance.Play(enterSE, SoundManager.Sound.UI);
    }
    //操作確認画面を閉じる
    private void Close_Explanation()
    {
        Title_Display.SetActive(true);
        Explanation.SetActive(false);
        explanation = false;

        //SoundManager.Instance.Play(exitSE, SoundManager.Sound.UI);
    }

    //ゲームを終了
    public void GameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        //SoundManager.Instance.Play(exitSE, SoundManager.Sound.UI);
        Application.Quit();
    }
}
