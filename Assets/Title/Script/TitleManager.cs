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
    //操作確認画面の表示状態
    bool explanation = false;
    //イベントシステム
    EventSystem eventSystem;

    void Start()
    {
        //最初は操作確認画面は非表示
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
            if (Explanation.activeSelf) Close_Explanation();
            else
            {
                //プログラムを終了
                GameExit();
            }
        }
    }

    //選択しているボタンのエフェクトを変える
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

    //シーンの読み込み
    private void LoadScene()
    {
        //SceneManager.LoadScene("Test_Map");
        LoadingSceneController.LoadScene("Test_Map");
    }
    //ゲームを開始
    public void GameStart()
    {
        //0.2秒後にシーンを切り替える
        //Invoke("LoadScene", 0.2f);
        LoadScene();

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
