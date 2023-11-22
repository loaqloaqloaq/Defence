using UnityEngine;

[System.Serializable]
public struct AudioData
{
    public int maxSENum;       // 同時最大発音数
    [Range(0.0f,1.0f)]
    public float initVolume;  // 1個目のボリューム

    public string name;     //SEName
    public string resourcePath; //リソースディレクトリ
    public float audioLength;   //音源の長さ

    public AudioClip clip;  //Audioclip File

    /*
    public AudioData(string resourcePath, string name, int maxSENum, float initVolume, float audioLength, AudioClip clip)
    {
        this.resourcePath = resourcePath;
        this.name = name;
        this.maxSENum = maxSENum;
        this.initVolume = initVolume;
        this.audioLength = audioLength;
        this.clip = clip;
    }
    */
}