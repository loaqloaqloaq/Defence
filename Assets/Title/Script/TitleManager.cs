using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//----------------------------------------
//タイトル管理
//----------------------------------------
public class TitleManager : MonoBehaviour
{
    //タイトル表示UI
    [SerializeField] private GameObject Title_Display;
    //今、選択しているボタン
    private GameObject nowSelectButton;
    //前、選択されていたボタン
    private GameObject prevSelectButton;
    //イベントシステム
    EventSystem eventSystem;

    void Start()
    {
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

        //"Submit"ボタン (Spaceキー)を押したら
        if (Input.GetButtonDown("Submit"))
        {
            nowSelectButton.GetComponent<Button>().onClick.Invoke();
        }
        //Escapeキーを押したら
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //プログラムを終了
            GameExit();
        }
    }

    //シーンの読み込み
    private void LoadScene()
    {
        LoadingSceneController.LoadScene("Test_Map");
    }

    //ゲームを開始
    public void GameStart()
    {
        //SoundManager.Instance.Play(enterSE, SoundManager.Sound.UI);
        //シーンの読み込み
        LoadScene();
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
