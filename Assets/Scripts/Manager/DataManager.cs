using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    //SIngleton
    private static DataManager instance;
    public static DataManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<DataManager>();
            }
            return instance;
        }
    }
    //PCにゲームデータをJson形式に保存する
    string GameDataFileName = "GameData.json"; //ファイルネーム

    public Data data;

    // データロード
    public void LoadGameData()
    {
        string filePath = Application.persistentDataPath + "/" + GameDataFileName;

        if (File.Exists(filePath))
        {
            string FromJsonData = File.ReadAllText(filePath);
            data = JsonUtility.FromJson<Data>(FromJsonData);
            //Debug.Log("Data Load Success");
        }
        else
        {
            data = new Data();
            data.Init();
            //Debug.Log("Data Load Failed");
        }
    }

    // PCにゲームデータをJson形式に保存する
    public void SaveGameData()
    {
        // クラスをJsonに変換
        string ToJsonData = JsonUtility.ToJson(data, true);
        string filePath = Application.persistentDataPath + "/" + GameDataFileName;

        //directoryにファイル保存
        File.WriteAllText(filePath, ToJsonData);

        //Debug.Log("Save Sucess");
    }

    //アプリケーションを終了時にセーブ
    private void OnApplicationQuit()
    {
        SaveGameData();
    }
}