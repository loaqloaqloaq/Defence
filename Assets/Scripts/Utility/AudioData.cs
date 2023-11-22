using UnityEngine;

[System.Serializable]
public struct AudioData
{
    public int maxSENum;       // �����ő唭����
    [Range(0.0f,1.0f)]
    public float initVolume;  // 1�ڂ̃{�����[��

    public string name;     //SEName
    public string resourcePath; //���\�[�X�f�B���N�g��
    public float audioLength;   //�����̒���

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