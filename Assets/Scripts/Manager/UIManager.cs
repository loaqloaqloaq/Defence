﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

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

    //UIオブジェクト
    [SerializeField] private GameObject gameoverUI; 
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject settingUI;
    [SerializeField] private Crosshair crosshair;
    [SerializeField] private Image staminaBackground;
    [SerializeField] private Image staminaImage;
    [SerializeField] private Image lifeImage;
    [SerializeField] private Image[] grenadeImages = new Image[3];
    [SerializeField] private Image[] weaponSlotImages = new Image[2];
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI lifeText;

    [SerializeField] private RectTransform aimPointRect;

    //クロスヘア＆エイムポイント
    private float crosshairSize = 0.7f;
    private float sizeLerpTime = 0.1f;
    private float aimPointStartSize;

    private float previousHealth = 100.0f;
    private float grownRate = 5.0f;
    private float currentScore;
    private float nextScore;

    private bool isUpdatingHealth;

    private Animator animator;

    public bool isPause { get; private set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        aimPointStartSize = aimPointRect.sizeDelta.x;
        currentScore = 0;
        nextScore = 0;
        SetMouseVisible(false);
        isPause = false;
    }

    private void Update()
    {
        if (GameManager.Instance.isGameover)
        {
            return;
        }

        UpdateScore();
        
        if (Input.GetKeyDown(KeyCode.Escape) && !settingUI.activeSelf)
        {
            if (!isPause)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }
    }

    //設定UI表示・非表示
    public void SetActiveSettingPanel(bool isActive)
    {
        settingUI.SetActive(isActive);
        if (isActive)
        {
            GetComponent<SettingManager>().LoadSettings();
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
        isPause = true;
        animator.Play("pause_Anim");
        StartCoroutine(SetPanel(true));
        Time.timeScale = 0;
    }
    public void Resume()
    {
        isPause = false;
        animator.Play("resume_Anim");
        StartCoroutine(SetPanel(false));
        Time.timeScale = 1.0f;
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
    public void UpdateAmmoText(int magAmmo, int remainAmmo, bool isInfinity)
    {
        if (isInfinity)
        {
            ammoText.text = magAmmo.ToString();
        }
        else
        {
            ammoText.text = magAmmo + " / " + remainAmmo;
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
        yield return new WaitForEndOfFrame();
        isUpdatingHealth = true;

        if (anim){ animator.SetTrigger("helath_Trigger"); }

        float growthRate = (previousHealth - currentHealth) * Time.deltaTime * 6.0f;
        float amount = 0;
        float duration = 0;
        
        while (previousHealth != currentHealth && duration <= 1.0f && isUpdatingHealth)
        {
            previousHealth -= growthRate;
            duration += Time.deltaTime;
            float gap = Mathf.Abs(currentHealth - previousHealth);
            if (gap < Mathf.Abs(growthRate * 3.0f))
            {
                previousHealth = currentHealth;
            }
            amount = previousHealth / maxHealth;
            lifeImage.fillAmount = amount;
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
    public void UpdateWeaponSlotImage(int weaponSlotindex, bool isOn)
    {
        if (weaponSlotindex < 0 || weaponSlotindex >= weaponSlotImages.Length)
        {
            return;
        }
        float alpha = isOn ? 1.0f : 100 / 255.0f;
        weaponSlotImages[weaponSlotindex].color = new Color(1, 1, 1, alpha);
    }

    //Waveクリア時のUIアニメーション再生
    public void WaveClear()
    {
        animator.SetTrigger("wave_Clear");
    }
}