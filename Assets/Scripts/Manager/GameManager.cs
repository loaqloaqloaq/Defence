﻿using System;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : MonoBehaviour
{
    enum GameState
    {
        NON, STARTED, END
    }
    GameState gameState;
    public InputDevice lastInputDevice;
    //Singleton       
    private static GameManager instance;

    [SerializeField] Timer timerScript;
    [SerializeField] ScrapUI scrapUI;
    [SerializeField] float playTime;
    [SerializeField] public int scrap;
    [SerializeField] TextMeshProUGUI scrapText;
    public int killCount;
    public int playerDamagedCount;
    public int usedScrap;
    public float timer;

    //スカイボックスを回転させる
    [Range(0.01f, 0.1f)]
    float originalRotate;
    public float rotateSpeed;
    public Material now_sky;
    private float rotationRepeatValue;
    //private float maxtime;  

    public int currentStage;

    public int NPCCount,MaxNPCCount;

    float endTime,endTimer;

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
        set { 
            if(Instance.lastInputDevice!=value) UIButton.SetUIText(value);
            Instance.lastInputDevice = value; 
        }
        get
        {   
            return Instance.lastInputDevice;
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
        UIButton.SetUIText(lastInputDevice);

        if (MaxNPCCount == 0) MaxNPCCount = 5;
        NPCCount = GameObject.FindGameObjectsWithTag("NPC").Length;

        if (now_sky) originalRotate = now_sky.GetFloat("_Rotation");

        gameState = GameState.STARTED;
        currentStage = 0;
        endTimer = 7f;
        endTime = 0;

    }
    private void Update()
    {
        if (timer <= 0 && gameState != GameState.END )
        {
            Debug.Log("勝利");
            End(1);
        }

        if (gameState == GameState.STARTED) TimerUpdate();
        if (gameState == GameState.END) {
            endTime += Time.deltaTime;
            //Debug.Log(endTime);
            if (endTime >= endTimer) { 
                ToResultScene();
            }
        }
        SkyRotation();       
    }
    private void OnDestroy()
    {
        //スカイボックスのマテリアルを元に戻す
        now_sky.SetFloat("_Rotation", originalRotate);
        
    }

    public void End(int type) {
        //ゲーム終了処理がした場合、繰り返しての執行を防ぐ
        if (gameState == GameState.END) return;
        //1時間切れ勝利 2敵拠点破壊勝利 3ゲート全破壊された　4プレイヤー死んだ       
        ShowEndSubtitle(type);
        if (type == 4) type = 3;
        Record.resultID = type;
        gameState = GameState.END;  
        if (type!=3) KillAllEnemy();        
        //Debug.Log(gameState);
    }

    void ShowEndSubtitle(int type) {
        sireiController.showText("End"+ type);
    }

    void KillAllEnemy() {
        EnemyGeneratorManager.Instance.GenerateEnemy = false;
        DamageMessage dm = new DamageMessage();
        dm.damager = gameObject;
        dm.amount = 9999;
        Transform pool = GameObject.Find("Enemy Pool").transform;
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        IEnemyDamageable[] allEnemey = pool.GetComponentsInChildren<IEnemyDamageable>();
        foreach(IEnemyDamageable enemy in allEnemey) {
            var dis = Vector3.Distance(player.position, enemy.transform.position);
            if(dis<100) enemy.ApplyDamage(dm);
            else enemy.gameObject.SetActive(false);
        }
    }

    void TimerUpdate() {
        timer -= Time.deltaTime;
        if (timer < 0) timer = 0;
        EnemyGeneratorManager.Instance?.ChangeMaxEnemy(timer / (playTime * 60));
        PlayerPrefs.SetFloat("clearTime", playTime * 60 - timer);
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
        PlayerPrefs.SetFloat("remainingTime", timer);
        SceneManager.LoadScene("Result");
    }
    
    //ゲームシーンをRestart
    public void Restart()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }
    public void Title() {
        SceneManager.LoadScene("Title");
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
        else {
            Debug.LogWarning("cannot found scrap UI");
        }
        usedScrap += amount;
    }
    public void SetScrap(int amount)
    {
        scrap = amount;
        if (scrap >= 999999) scrap = 999999;
        else if (scrap <= 0) scrap = 0;
        if (scrapUI) scrapUI.SetScrapText();
        usedScrap += amount;
    }
    private void SkyRotation()
    {
        rotationRepeatValue = Mathf.Repeat(now_sky.GetFloat("_Rotation") + rotateSpeed, 360f);

        now_sky.SetFloat("_Rotation", rotationRepeatValue);
    }
    public bool isNPCFull() {
        return NPCCount >= MaxNPCCount;
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