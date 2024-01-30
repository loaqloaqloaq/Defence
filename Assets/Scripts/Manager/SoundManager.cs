using System.Collections.Generic;
using UnityEngine;
using static AudioClipInfo;

public class SoundManager : MonoBehaviour
{
    //Singleton
    private static SoundManager instance;
    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SoundManager>();
            }
            return instance;
        }
    }

    public enum Sound
    {
        BGM,
        EFFECT,
        UI,
        MaxCount,  //Length
    }

    //音を鳴らすAudisoSourceを複数所持
    [SerializeField] AudioSource[] audioSources = new AudioSource[(int)Sound.MaxCount];
    //directoryと一緒にサウンドリソースをDictionaryに保存
    Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    Dictionary<string, AudioClipInfo> audioClipInfo = new Dictionary<string, AudioClipInfo>();

    private List<AudioSource> seList = new List<AudioSource>();

    private float currentVolume;

    private bool IsContained(string seName)
    { 
        return audioClipInfo.ContainsKey(seName);
    }


    private void Start()
    {
        seList.Add(audioSources[(int)Sound.EFFECT]);  
    }

    public void Clear()
    {
        // すべてのAudioSource Stop
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        // サウンドリソース削除
        audioClipInfo.Clear();
        audioClips.Clear();
    }

    private void Update()
    {
        update();
    }

    public void update()
    {
        // playing SE update
        foreach (AudioClipInfo info in audioClipInfo.Values)
        {
            List<SEInfo> newList = new List<SEInfo>();

            foreach (SEInfo seInfo in info.playingList)
            {
                seInfo.curTime = seInfo.curTime - Time.deltaTime;
                if (seInfo.curTime > 0.0f)
                    newList.Add(seInfo);
                else
                    info.stockList.Add(seInfo.index, seInfo);
            }
            info.playingList = newList;
        }

    }

    //SE再生
    public void PlaySE(string seName, AudioSource adSource = null, float pitch = 1.0f)
    {
        AudioClipInfo acInfo = GetAudioInfo(seName); //リソースを取得
        if (acInfo == null)
        {
            Debug.Log("Audio Info is null");
            return;
        }

        //AudioClipInfoのClipが割り当てられなかったらリソースロード
        if (acInfo.clip == null)
        {
            acInfo.clip = (AudioClip)Resources.Load(acInfo.resourcePath);
            if (acInfo.clip == null)
                Debug.LogWarning("SE is not found.");
        }

        float len = acInfo.audioLength;

        //Stockの残りの数が０だったらSEを再生しない
        if (acInfo.stockList.Count > 0)
        {
            SEInfo seInfo = acInfo.stockList.Values[0];
            seInfo.curTime = len;
            acInfo.playingList.Add(seInfo);

            // Stockから除去
            acInfo.stockList.Remove(seInfo.index);
            // SE再生
            if (adSource != null)
            {
                adSource.pitch = pitch;
                adSource.PlayOneShot(acInfo.clip, seInfo.volume);
            }
            else
            {
                AudioSource audioSource = audioSources[(int)Sound.EFFECT];
                audioSource.pitch = pitch;
                audioSource.PlayOneShot(acInfo.clip, seInfo.volume);
            }

            //Debug.Log(seInfo.volume);
        }
    }

    //サウンド再生(BGM・UI）
    public void Play(string path, Sound type = Sound.UI, float pitch = 1.0f)
    {
        AudioClip audioClip = GetAudio(path);

        if (type == Sound.BGM) //Background music
        {
            AudioSource audioSource = audioSources[(int)Sound.BGM];
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else if (type == Sound.UI) // UI Sosund
        {
            AudioSource audioSource = audioSources[(int)Sound.UI];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
        }
    }

    //サウンドリソース取得
    AudioClipInfo GetAudioInfo(string seName)
    {
        if (!IsContained(seName))
            return null;//既に登録されている音を返す

        AudioClipInfo info = audioClipInfo[seName];

        //AudioClipInfoのClipが割り当てられなかったらリソースロード
        if (info.clip == null)
        {
            info.clip = (AudioClip)Resources.Load(info.resourcePath);
            if (info.clip == null)
                Debug.LogWarning("SE" + seName + "is not found.");
            else
                return audioClipInfo[seName];
        }

        return audioClipInfo[seName];
    }

    public AudioClip GetAudio(string path)
    {
        if (path.Contains("Sounds/") == false)
            path = $"Sounds/{path}"; // directoryに"Sounds/"が含まれていない場合追加

        AudioClip audioClip = null;

        //すでに取得済みのサウンドか確認
        if (audioClips.TryGetValue(path, out audioClip) == false)
        {
            audioClip = Resources.Load<AudioClip>(path);
            audioClips.Add(path, audioClip);
        }

        if (audioClip == null)
            Debug.Log($"AudioClip Missing ! {path}");

        return audioClip;
    }

    public bool AddAudioInfo(string seName, AudioClip clip, int maxSE, float length, float initVolume = 1.0f, string path = "Sounds/")
    {
        //登録されているAudioInfoの重複を避ける
        if (IsContained(seName)) return false;

        AudioClipInfo newAcInfo = new AudioClipInfo(path, seName, maxSE, initVolume, length);
        newAcInfo.clip = clip;

        //ClipInfoに追加して管理
        audioClipInfo.Add(newAcInfo.name, newAcInfo);

        return true;
    }

    public bool AddAudioInfo(AudioData audioData)
    {
        //登録されているAudioInfoの重複を避ける
        if (IsContained(audioData.name)) return false;

        if (audioData.clip == null)
        {
            Debug.Log($"AudioClip Missing!");
            return false;
        }

        AudioClipInfo newAcInfo = new AudioClipInfo(audioData.resourcePath, audioData.name, audioData.maxSENum, audioData.initVolume, audioData.audioLength);
        newAcInfo.clip = audioData.clip;

        //ClipInfoに追加して管理
        audioClipInfo.Add(newAcInfo.name, newAcInfo);

        return true;
    }

    //Volume Setting 
    public void SetMusicVolume(float volume)
    {
        audioSources[(int)Sound.BGM].volume = volume;
    }
    public void SetSfxVolume(float volume)
    {
        currentVolume = volume;
        int index = 0;
        foreach (AudioSource source in seList)
        {
            if (source == null)
            {
                //seList.RemoveAt(index);
                continue;
            }
            index++;
            source.volume = volume;
        }
    }

    public void AddSESource(AudioSource ad)
    {
        seList.Add(ad);
        ad.volume = currentVolume;
    }
}


