using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private AudioSource audioPlayer;

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

    public void Clear()
    {
        // すべてのAudioSource Stop
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        // サウンドリソース削除
        audioClips.Clear();
    }

    //directoryを受けてこのスクリプトの内で再生させる
    public void Play(string path, Sound type = Sound.EFFECT, float pitch = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type); //リソースを取得
        Play(audioClip, type, pitch);
    }

    //サウンド再生（リソースあり）
    public void Play(AudioClip audioClip, Sound type = Sound.EFFECT, float pitch = 1.0f)
    {
        if (audioClip == null)
            return;

        if (type == Sound.BGM) //Background music
        {
            AudioSource audioSource = audioSources[(int)Sound.BGM];
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else if (type == Sound.EFFECT) // Effect Sound
        {
            AudioSource audioSource = audioSources[(int)Sound.EFFECT];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
        }
        else if (type == Sound.UI) // UI Sosund
        {
            AudioSource audioSource = audioSources[(int)Sound.UI];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
        }
    }

    //サウンドリソース取得
    AudioClip GetOrAddAudioClip(string path, Sound type = Sound.EFFECT)
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

    //Volume Setting 
    public void SetMusicVolume(float volume)
    {
        audioSources[(int)Sound.BGM].volume = volume;
    }
    public void SetSfxVolume(float volume)
    {
        audioSources[(int)Sound.EFFECT].volume = volume;
    }
}


