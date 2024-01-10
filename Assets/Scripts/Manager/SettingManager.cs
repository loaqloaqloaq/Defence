using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingManager : MonoBehaviour
{
    //Singleton
    public static SettingManager Instance
    {
        get
        {
            if (instance = null)
            {
                instance = FindObjectOfType<SettingManager>();
            }
            return instance;
        }
    }
    private static SettingManager instance;

    //フルスクリーンボタン
    public Toggle fullScreenBtn;
    
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider msSensitivitySlider;

    //Volume, Camera Speed 
    private float musicVolume;
    private float sfxVolume;
    [SerializeField] private float xAxis;
    [SerializeField] private float yAxis;

    //カメラ設定の最大・最小値
    private const float minXAxis = 100.0f;
    private const float maxXAxis = 500.0f;
    private const float minYAxis = 100.0f;
    private const float maxYAxis = 250.0f;

    [SerializeField] private TextMeshProUGUI msSliderValueText;
    [SerializeField] private TextMeshProUGUI bgmSliderValueText;
    [SerializeField] private TextMeshProUGUI sfxSliderValueText;

    private void Awake()
    {
        msSensitivitySlider.onValueChanged.AddListener(UpdateSensitivtyValueText);
        msSensitivitySlider.onValueChanged.AddListener(SetCameraXAxis);
        msSensitivitySlider.onValueChanged.AddListener(SetCameraYAxis);

        bgmSlider.onValueChanged.AddListener(SetMusicVolume);
        bgmSlider.onValueChanged.AddListener(UpdateBGMValueText);
        sfxSlider.onValueChanged.AddListener(SetSfxVolume);
        sfxSlider.onValueChanged.AddListener(UpdateSFXValueText);
    }

    private void UpdateSensitivtyValueText(float value)
    {
        msSliderValueText.text = (value * 100.0f).ToString("F0"); 
    }
    private void UpdateBGMValueText(float value)
    {
        bgmSliderValueText.text = (value * 100.0f).ToString("F0");
    }
    private void UpdateSFXValueText(float value)
    {
        sfxSliderValueText.text = (value * 100.0f).ToString("F0");
    }

    void Start()
    {
        InitSetting();
    }


    //Set BGM, Sfx volume
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        SoundManager.Instance.SetMusicVolume(musicVolume);
    }
    public void SetSfxVolume(float volume)
    {
        sfxVolume = volume;
        SoundManager.Instance.SetSfxVolume(sfxVolume);
    }

    //Set Camera X,Y Speed
    public void SetCameraXAxis(float x)
    {
        xAxis = minXAxis + (maxXAxis - minXAxis) * x;
    }
    public void SetCameraYAxis(float y)
    {
        yAxis = minYAxis + (maxYAxis - minYAxis) * y;
    }

  
    //確認ボタン
    public void OKButtonClicked()
    {
        var data = DataManager.Instance.data;

        //カメラセッティング
        SetPlayerCameraAxis();

        data.SetCameraSetting(xAxis, yAxis);
        data.SetVolumeSetting(musicVolume, sfxVolume);

        DataManager.Instance.SaveGameData();
    }

    //キャラクタのカメラ速度設定
    private void SetPlayerCameraAxis()
    {
        var charAim = FindObjectOfType<PlayerAiming>();
        if (charAim)
        {
            charAim.SetAxisSpeed(xAxis, yAxis);
        }
    }

    //初期化
    private void InitSetting()
    {
        LoadSettings();
    }

    //PCに保存されている既存の設定値をロードする　
    public void LoadSettings() 
    {
        //データマネージャーからロード
        DataManager.Instance?.LoadGameData();
        var data = DataManager.Instance.data;
        //無ければロードしない
        if (data == null)
        {
            return;
        }

        //Camera
        xAxis = data.data[(int)Data.dataType.xAxis];
        yAxis = data.data[(int)Data.dataType.yAxis];
        SetPlayerCameraAxis();
        msSensitivitySlider.value = (xAxis - minXAxis) / (maxXAxis - minXAxis);
        UpdateSensitivtyValueText(msSensitivitySlider.value);

        //Volume
        musicVolume = data.data[(int)Data.dataType.musicVolume];
        sfxVolume = data.data[(int)Data.dataType.sfxVolume];
        SetMusicVolume(musicVolume);
        SetSfxVolume(sfxVolume);
        UpdateBGMValueText(bgmSlider.value);
        UpdateSFXValueText(sfxSlider.value);
        bgmSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;
    }
}
