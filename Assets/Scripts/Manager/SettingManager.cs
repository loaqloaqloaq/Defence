using System.Collections.Generic;
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

    public TMP_Dropdown resolutionDropdown;
    //フルスクリーンボタン
    public Toggle fullScreenBtn;
    //解像度リスト
    List<Resolution> resolutions = new List<Resolution>();
    FullScreenMode screenMode;
    
    private int resolutionNum;
    
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider xAxisSlider;
    [SerializeField] Slider yAxisSlider;

    //Volume, Camera Speed 
    private float musicVolume;
    private float sfxVolume;
    private float xAxis;
    private float yAxis;

    //カメラ設定の最大・最小値
    private const float minXAxis = 150.0f;
    private const float maxXAxis = 450.0f;
    private const float minYAxis = 150.0f;
    private const float maxYAxis = 450.0f;

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

    //UI 
    public void ResolutuinDropboxOptionChange(int index)
    {
        resolutionNum = index;
    }

    //確認ボタン
    public void OKButtonClicked()
    {
        //var data = DataManager.Instance.data;
        //解像度
        /*
        Screen.SetResolution(resolutions[resolutionNum].width,
            resolutions[resolutionNum].height,
            screenMode);
        */
        //カメラセッティング
        SetPlayerCameraAxis();
        //data.SetCameraSetting(xAxis, yAxis);
        //data.SetVolumeSetting(musicVolume, sfxVolume);

        //DataManager.Instance.SaveGameData();
    }

    //キャラクタのカメラ速度設定
    private void SetPlayerCameraAxis()
    {
        var charAim = FindObjectOfType<PlayerAiming>();
        if (charAim)
        {
            charAim.xAxis.m_MaxSpeed = xAxis;
            charAim.yAxis.m_MaxSpeed = yAxis;
        }
    }

    public void FullScreenBtn(bool isFull)
    {
        screenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    //初期化
    private void InitSetting()
    {
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            resolutions.Add(Screen.resolutions[i]);
            /*
            if (Screen.resolutions[i].refreshRate == 60)
            {
                //
            }
            */
        }
        resolutionDropdown.options.Clear();

        //解像度ドロップダウンのオプション初期化
        int optionNum = 0;
        foreach (var resolution in resolutions)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = resolution.width + " x " + resolution.height + " " + resolution.refreshRateRatio + "";
            resolutionDropdown.options.Add(option);

            if (resolution.width == Screen.width && resolution.height == Screen.height)
            {
                resolutionDropdown.value = optionNum;
            }
            ++optionNum;
        }
        resolutionDropdown.RefreshShownValue();
        fullScreenBtn.isOn = Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow) ? true : false;

        //LoadSettings();
    }

    //PCに保存されている既存の設定値をロードする　
    public void LoadSettings() 
    {
        //temp
        return;
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
        yAxis = data.data[(int)Data.dataType.xAxis];
        SetPlayerCameraAxis();
        xAxisSlider.value = (xAxis - minXAxis) / (maxXAxis - minXAxis);
        yAxisSlider.value = (yAxis - minYAxis) / (maxYAxis - minYAxis);

        //Volume
        musicVolume = data.data[(int)Data.dataType.musicVolume];
        sfxVolume = data.data[(int)Data.dataType.sfxVolume];
        SetMusicVolume(musicVolume);
        SetSfxVolume(sfxVolume);
        bgmSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;
    }
}
