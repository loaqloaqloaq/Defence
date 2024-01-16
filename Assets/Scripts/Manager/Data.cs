using System;

[Serializable] 
public class Data
{
    //設定値のタイプ
    public enum dataType
    {
        musicVolume,
        sfxVolume,
        xAxis,
        yAxis,
        highScore,
        MaxCount
    };

    public float[] data = new float[10];　
    
    //最高記録保存
    public void SetHighScore(int score)
    {
        data[(int)dataType.highScore] = (float)score;
    }

    //カメラセッティング保存
    public void SetCameraSetting(float x, float y)
    {
        data[(int)dataType.xAxis] = x;
        data[(int)dataType.yAxis] = y;
    }

    //ボリュームセッティング保存
    public void SetVolumeSetting(float mVolue, float sVolue)
    {
        data[(int)dataType.musicVolume] = mVolue;
        data[(int)dataType.sfxVolume] = sVolue;
    }

    //初期化
    public void Init()
    {
        data[(int)dataType.musicVolume] = 1.0f;
        data[(int)dataType.sfxVolume] = 1.0f;
        data[(int)dataType.xAxis] = 300.0f;
        data[(int)dataType.yAxis] = 175.0f;
    }
}
