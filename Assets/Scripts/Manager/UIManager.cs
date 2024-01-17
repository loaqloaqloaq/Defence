using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<UIManager>();

            return instance;
        }
    }
    //Canvas 
    [SerializeField] Canvas canvas_Player;
    [SerializeField] Canvas canvas_Sniping;
    [SerializeField] Canvas canvas_UI;

    //UIオブジェクト
    [SerializeField] private GameObject gameoverUI; 
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject settingUI;
    [SerializeField] private GameObject ammoPanel;
    [SerializeField] private Crosshair crosshair;
    [SerializeField] private Image staminaBackground;
    [SerializeField] private Image staminaImage;
    [SerializeField] private Image HealthBar;
    [SerializeField] private Image[] grenadeImages = new Image[3];
    [SerializeField] private Image[] weaponSlotImages = new Image[2];
    [SerializeField] private TextMeshProUGUI[] weaponSlotText = new TextMeshProUGUI[4];
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI remainAmmoText;
    [SerializeField] private TextMeshProUGUI magAmmoText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI lifeText;

    [SerializeField] private RectTransform aimPointRect;

    private EventSystem eventSystem;
    [SerializeField] private GameObject firstSelectedButton;
    [SerializeField] private GameObject settingFirstButton;

    private Color slotTextColor = new Vector4(1.0f, 1.0f, 1.0f, 0.56f);

    //クロスヘア＆エイムポイント
    private float crosshairSize = 0.7f;
    private float sizeLerpTime = 0.1f;
    private float aimPointStartSize;

    private float previousHealth = 800.0f;
    private float grownRate = 5.0f;
    private float currentScore;
    private float nextScore;

    private bool isUpdatingHealth;

    private Animator animator;

    private PlayerInput playerInput;

    private SettingManager settingManager;
    
    public bool isPause { get; private set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        eventSystem = FindObjectOfType<EventSystem>();
        playerInput = FindObjectOfType<PlayerInput>();
        settingManager = GetComponent<SettingManager>();
        aimPointStartSize = aimPointRect.sizeDelta.x;
        currentScore = 0;
        nextScore = 0;
        SetMouseVisible(false);
        isPause = false;
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown("joystick button 7")) 
            && !settingUI.activeSelf)
        {
            if (!isPause) { Pause();} 
            else { Resume(); }
        }
    }


    private void SetSelectedButton(GameObject select)
    {
        if (select == null) return;
        if (playerInput) 
        {
            if (!playerInput.controllerConnected) return; 
        }

        eventSystem.SetSelectedGameObject(select);
    }

    //設定UI表示・非表示
    public void SetActiveSettingPanel(bool isActive)
    {
        settingUI.SetActive(isActive);
        if (isActive)
        {
            SetSelectedButton(settingFirstButton);
            settingManager.LoadSettings();
        }
        else
        {
            SetSelectedButton(firstSelectedButton);
        }
    }
    //マウスカーソル表示・非表示
    public void SetMouseVisible(bool isVisible)
    {
        Cursor.visible = isVisible;
        Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;
    }

    //一時停止&再開
    public void Pause()
    {
        var turretUI = TurretUI.Instance;
        var shopUI = ShopUI.Instance;
        var map= MapController.Instance;
        
        if (turretUI?.isOpened  == true)    turretUI.CloseUI(); 
        if (shopUI?.isOpened    == true)    shopUI.CloseUI();
        if (map?.enable         == true)    map.SetVisible(false);

        isPause = true;
        animator.Play("pause_Anim");
        SetSelectedButton(firstSelectedButton);
        StartCoroutine(SetPanel(true));
        Time.timeScale = 0;
        playerInput.enabled = false;
    }
    public void Resume()
    {
        isPause = false;
        animator.Play("resume_Anim");
        StartCoroutine(SetPanel(false));
        Time.timeScale = 1.0f;
        playerInput.enabled = true;
    }

    public void Restart()
    {
        GameManager.Instance?.Restart();
    }

    public void Title()
    {
        GameManager.Instance?.Title();
    }

    //Pause UI表示
    IEnumerator SetPanel(bool isActive)
    {
        //効果音
        SoundManager.Instance.Play("Sounds/UI_Sfx/Click_Electronic_Pause", SoundManager.Sound.UI);

        while (animator.GetCurrentAnimatorStateInfo(2).normalizedTime < 1.0f)
        {
            yield return new WaitForEndOfFrame();
        }
        SetMouseVisible(isActive);
    }

    //グレネードUI
    public void UpdateGrenadeImage(int remainGrenade)
    {
        for (int i = 0; i < grenadeImages.Length; ++i)
        {
            bool isAcitve = i < remainGrenade;
            grenadeImages[i].enabled = isAcitve;
        }
    }
    //弾の数表示
    public void UpdateAmmoText(int magAmmo, int remainAmmo, bool isInfinity, bool isHolstered)
    {
        if (isHolstered)
        {
            ammoPanel.SetActive(false);
            return;
        }

        if (!ammoPanel.activeSelf) ammoPanel.SetActive(true);

        if (isInfinity)
        {
            magAmmoText.text = magAmmo.ToString();
            remainAmmoText.text = "∞";
        }
        else
        {
            magAmmoText.text = magAmmo.ToString();
            remainAmmoText.text = remainAmmo.ToString();
        }
    }

    //点数更新
    public void SetNextScore(int newScore)
    {
        animator.SetTrigger("renew_Score");
        nextScore = newScore;
    }
    private void UpdateScore()
    {
        currentScore += grownRate;
        if (currentScore > nextScore)
        {
            currentScore = nextScore;
        }
        scoreText.text = "Score : " + currentScore;
    }

    //Wave表示、残り敵の数
    public void UpdateWaveText(int waves, int count)
    {
        waveText.color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        waveText.text = "Wave : " + waves + "\nEnemy Left : " + count;
    }

    //体力ゲージ更新
    public void UpdateHealth(float maxHealth, float currentHealth, bool anim = false) //체력 
    {
        //
        if (isUpdatingHealth)
        { 
            isUpdatingHealth = false;
        }
        StartCoroutine(UpdateHealthGauge(maxHealth, currentHealth, anim));
    }
    //体力の実時間変化処理
    IEnumerator UpdateHealthGauge(float maxHealth, float currentHealth, bool anim)
    {
        //yield return new WaitForEndOfFrame();
        isUpdatingHealth = true;

        if (anim){ animator.SetTrigger("helath_Trigger"); }

        float growthRate = (previousHealth - currentHealth) * Time.deltaTime * 20.0f;
        float amount = 0;
        //float duration = 0;
        
        while (previousHealth != currentHealth && isUpdatingHealth) //&& duration >= 1.0f 
        {
            previousHealth -= growthRate;
            //duration += Time.deltaTime;
            float gap = Mathf.Abs(currentHealth - previousHealth);
            if (gap < Mathf.Abs(growthRate * 3.0f))
            {
                previousHealth = currentHealth;
            }
            amount = previousHealth / maxHealth;
            HealthBar.fillAmount = amount;
            Debug.Log(previousHealth + " "+ currentHealth + " " + amount);
            yield return new WaitForEndOfFrame();
        }
    }

    //スタミナの実時間変化処理
    public void UpdateStaminaGauge(float maxStamina, float currentStamina) 
    {
        if (currentStamina > maxStamina || currentStamina < 0)
        {
            return;
        }
        float value = currentStamina / maxStamina;
        staminaImage.fillAmount = value;
    }
    //スタミナUI表示・非表示
    public void SetActiveStamina(bool isActive)
    {
        staminaBackground.gameObject.SetActive(isActive);
    }

    //ゲームオーバPanel表示・非表示
    public void SetActiveGameoverUI(bool isActive)
    {
        gameoverUI.SetActive(isActive);
    }

    //残りライフの表示
    public void UpdateLifeText(int remainLife)
    {
        lifeText.text = "LIFE : " + remainLife.ToString() + " / 3"; 
    }

    //クロスヘアイメージをゲーム空間の座標から２D画面上に変換して表示
    public void UpdateCrossHairPosition(Vector3 worldPosition)
    {
        crosshair.UpdatePosition(worldPosition);
    }
    //クロスヘアイメージ表示・非表示
    public void SetActiveCrosshair(bool isActive)
    {
        crosshair.SetActiveCrosshair(isActive);
    }
    //ズームしているとクロスヘアのサイズをより小さくする
    public void SetCrosshairSize(bool isAiming)
    {
        if (isAiming)
        {
            var targetSize = new Vector2(aimPointStartSize, aimPointStartSize) * crosshairSize;
            aimPointRect.sizeDelta = Vector2.Lerp(aimPointRect.sizeDelta, targetSize, sizeLerpTime);
        }
        else
        {
            var targetSize = new Vector2(aimPointStartSize, aimPointStartSize);
            aimPointRect.sizeDelta = Vector2.Lerp(aimPointRect.sizeDelta, targetSize, sizeLerpTime);
        }
    }

    //現在使っている武器のUI表示
    public void UpdateWeaponSlotImage(int weaponSlotindex)
    {
        if (weaponSlotindex < 0) return;
       
        for (int i = 0; i < weaponSlotImages.Length; i++)
        {
            weaponSlotImages[i].enabled = (i == weaponSlotindex);
            weaponSlotText[i].color = i == weaponSlotindex ? Color.white : slotTextColor;
        }
    }


    //Waveクリア時のUIアニメーション再生
    public void WaveClear()
    {
        animator.SetTrigger("wave_Clear");
    }

    public void SetEnableCanvas_Player(bool isEnable)
    {
        canvas_Player.enabled = isEnable;
    }

    public void SetEnableCanvas_UI(bool isEnable)
    {
        canvas_UI.enabled = isEnable;
    }

    public void SetEnableCanvas_Sniping(bool isEnable)
    {
        canvas_Sniping.enabled = isEnable;
    }

    public void UpdateWeaponSlot(int index, bool isActive)
    {
        if (index < 0 || index >= weaponSlotText.Length) return;
        weaponSlotText[index].GetComponentInChildren<Image>().enabled = isActive;
    }
}