using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using static AudioClipInfo;

public class SoundManager_ : MonoBehaviour
{
    Dictionary<string, AudioClipInfo> audioClips = new Dictionary<string, AudioClipInfo>();

    [SerializeField] private AudioSource audioSource;
    [SerializeField] AudioClip clip;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        AudioClipInfo acc = new AudioClipInfo("path", "shot", 10, 1.0f, 1 / 25.0f);
        acc.clip = clip;
        audioClips.Add("shot", acc);
    }


    private void Update()
    {
        update();
    }

    public bool playSE(string seName)
    {
        if (audioClips.ContainsKey(seName) == false)
            return false;//not register

        AudioClipInfo info = audioClips[seName];
  
        //AudioClipInfoのClipが割り当てられなかったらリソースロード
        if (info.clip == null)
        {
            info.clip = (AudioClip)Resources.Load(info.resourcePath);
            if (info.clip == null)
                Debug.LogWarning("SE" + seName + "is not found.");
        }

        float len = info.audioLength;

        //Stockの残りの数が０だったらSEを再生しない
        if (info.stockList.Count > 0)
        {
            SEInfo seInfo = info.stockList.Values[0];
            seInfo.curTime = len;
            info.playingList.Add(seInfo);

            // Stockから除去
            info.stockList.Remove(seInfo.index);

            // SE再生
            audioSource.PlayOneShot(info.clip, seInfo.volume);
            //Debug.Log(seInfo.volume);
            return true;
        }
        return false;
    }

    public void update()
    {
        // playing SE update
        foreach (AudioClipInfo info in audioClips.Values)
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

}
