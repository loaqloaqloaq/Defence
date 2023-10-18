using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Singleton
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<GameManager>();

            return instance;
        }
    }

    public bool isGameover { get; private set; }

    private void Awake()
    {
        Time.timeScale = 1.0f;
        if (Instance != this) Destroy(gameObject); 
        Record.Init();
    }
    public void EndGame()
    {
        isGameover = true;
        UIManager.Instance.SetActiveGameoverUI(true);
        UIManager.Instance.SetMouseVisible(true);
        UIManager.Instance.SetActiveCrosshair(false);
    }

    //結果シーンに移る
    public void ToResultScene()
    {
        SceneManager.LoadScene("ResultScene");
    }

    //タイトルシーンに移る
    public void ReturnToTitle()
    {
        SceneManager.LoadScene("Title");
    }

    //ゲームシーンをRestart
    public void Restart()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        SceneManager.LoadScene(sceneName);
    }

    //点数更新
    public void AddScore(int newScore)
    {
        if (!isGameover)
        {

            Record.score += newScore;
            UIManager.Instance.SetNextScore(Record.score);
        }
    }

    //獲得したアイテムの数を増やす
    public void AddItemCount()
    {
        ++Record.itemCount;
    }

    //倒した敵の数を増やす
    public void AddKillCount()
    {
        ++Record.killCount;
    }
}