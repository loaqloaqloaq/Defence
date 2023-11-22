using System.Collections.Generic;
using UnityEngine;

public class AudioClipInfo
{
    public int maxSENum = 10;        // 同時最大発音数
    public float initVolume = 1.0f;  // 1個目のボリューム
    public float attenuate = 0.89f;   // 合成時減衰率

    public AudioClip clip;
    public string resourcePath { get; private set; }
    public string name { get; private set; }
    public float audioLength { get; private set; }

    //Init
    public AudioClipInfo(string resourcePath, string name, int maxSENum, float initVolume, float audioLength)
    {
        this.resourcePath = resourcePath;
        this.name = name;
        this.maxSENum = maxSENum;
        this.initVolume = initVolume;
        this.audioLength = audioLength;
        //attenuate = calcAttenuateRate();

        // create stock list
        for (int i = 0; i < maxSENum; i++)
        {
            SEInfo seInfo = new SEInfo(i, 0.0f, initVolume * Mathf.Pow(attenuate, i));

            //SEInfo seInfo = new SEInfo(i, 0.0f, initVolume * (1 - i * 0.03f));
            stockList.Add(seInfo.index, seInfo);
        }

        this.audioLength = audioLength;
    }

    //SEInfoを使用してStockで管理
    public class SEInfo
    {
        public int index;
        public float curTime;
        public float volume;
        public SEInfo(int index, float curTime, float volume)
        {
            this.index = index;
            this.curTime = curTime; 
            this.volume = volume;   
        }
    }

    public SortedList<int, SEInfo> stockList = new SortedList<int, SEInfo>();
    public List<SEInfo> playingList = new List<SEInfo>();

    //音の減衰率啓啓さん
    float calcAttenuateRate()
    {
        float n = maxSENum;
        return NewtonMethod.run(
            delegate (float p) {
                return (1.0f - Mathf.Pow(p, n)) / (1.0f - p) - 1.0f / initVolume;
            },
            delegate (float p) {
                float ip = 1.0f - p;
                float t0 = -n * Mathf.Pow(p, n - 1.0f) / ip;
                float t1 = (1.0f - Mathf.Pow(p, n)) / ip / ip;
                return t0 + t1;
            },
            0.9f, 100
        );
    }
}
