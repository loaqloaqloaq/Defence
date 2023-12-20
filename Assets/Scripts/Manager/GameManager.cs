using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Singleton       
    private static GameManager instance;

    [SerializeField] Timer timerScript;
    [SerializeField] ScrapUI scrapUI;
    [SerializeField] float playTime;
    [SerializeField] public int scrap;

    [SerializeField] public float timer;

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
    private void Start()
    {
       timer = playTime * 60;
    }
    private void Update()
    {

        if (timer <= 0)
        {
            Record.result = "Failed";
            ToResultScene();
        }
        else TimerUpdate();
    }

    void TimerUpdate() {
        timer -= Time.deltaTime;
        if(timerScript) timerScript.setTimerString(timer);
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
        SceneManager.LoadScene("Result");
    }

    //タイトルシーンに移る
    public void ReturnToTitle()
    {
        //SceneManager.LoadScene("Title");
    }

    //ゲームシーンをRestart
    public void Restart()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }

    public void AddScrap(int amount) { 
        scrap+=amount;
        if (scrap >= 999999) scrap = 999999;
        if (scrapUI) scrapUI.SetScrapText();
    }

    public void DeductScrap(int amount){
        scrap -= amount;
        if (scrap <= 0) scrap = 0;
        if (scrapUI) scrapUI.SetScrapText();
    }
}