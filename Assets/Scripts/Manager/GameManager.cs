using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum InputDevice{
        KEYBOARD,GAMEPAD
    };
    public InputDevice lastInputDevice;
    //Singleton       
    private static GameManager instance;

    [SerializeField] Timer timerScript;
    [SerializeField] ScrapUI scrapUI;
    [SerializeField] float playTime;
    [SerializeField] public int scrap;
    public int killCount;
    public int playerDamagedCount;
    public int usedScrap;
    public float timer;

    //スカイボックスを回転させる
    [Range(0.01f, 0.1f)]
    public float rotateSpeed;
    public Material now_sky;
    private float rotationRepeatValue;
    //private float maxtime;
    List<string> GamePadKey=new List<string>();


    public static GameManager Instance
    {        
        get
        {
            if (instance == null) instance = FindObjectOfType<GameManager>();

            return instance;
        }
    }
    public static InputDevice LastInputDevice
    {
        get
        {   
            return Instance.lastInputDevice;
        }
    }

    public static bool IsNight
    {
        get
        {
            return FindObjectOfType<SunController>().isNight;
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
        PlayerPrefs.DeleteAll();
        timer = playTime * 60;
        killCount = 0;
        playerDamagedCount = 0;
        usedScrap = 0;
        //maxtime = timer;

        lastInputDevice = InputDevice.KEYBOARD; 
        
    }
    private void Update()
    {
        if (timer <= 0)
        {
            Debug.Log("勝利");
            Record.resultID = 1;
            ToResultScene();
        }
        else TimerUpdate();

        SkyRotation();

        //入力デバイス
        if (Input.anyKeyDown)
        {
             if(Input.inputString=="" && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1)) lastInputDevice = InputDevice.GAMEPAD;
             else lastInputDevice = InputDevice.KEYBOARD;

            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) {
                lastInputDevice = InputDevice.GAMEPAD;
            }
        }
    }

    void TimerUpdate() {
        timer -= Time.deltaTime;
        EnemyGeneratorManager.Instance?.ChangeMaxEnemy(timer / (playTime * 60));
        PlayerPrefs.SetFloat("timer", playTime * 60 - timer);
        if (timerScript) timerScript.setTimerString(timer);
    }
    public void EndGame()
    {
        isGameover = true;
        UIManager.Instance?.SetActiveGameoverUI(true);
        UIManager.Instance?.SetMouseVisible(true);
        UIManager.Instance?.SetActiveCrosshair(false);
    }

    //結果シーンに移る
    public void ToResultScene()
    {
        PlayerPrefs.SetInt("killCount",killCount);
        PlayerPrefs.SetInt("playerDamagedCount", playerDamagedCount);
        PlayerPrefs.SetInt("usedScrap", usedScrap);
        SceneManager.LoadScene("Result");
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
        usedScrap += amount;
    }
    private void SkyRotation()
    {
        rotationRepeatValue = Mathf.Repeat(now_sky.GetFloat("_Rotation") + rotateSpeed, 360f);

        now_sky.SetFloat("_Rotation", rotationRepeatValue);
    }
}

[Serializable]
public class SerializableKeyPair<TKey, TValue>
{
    [SerializeField] private TKey key;
    [SerializeField] private TValue value;

    public TKey Key => key;
    public TValue Value => value;
}