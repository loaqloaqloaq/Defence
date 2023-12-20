using System.Diagnostics.Tracing;

static public class Record 
{
    //プレイ中に点数とかかわるすべてをこのクラスに追加
    static public int score;
    static public int killCount;
    static public int itemCount;

    static public string result; //結果 (Clear or Failed)

    static public void Init()
    {
        score = 0;
        killCount = 0;
        itemCount = 0;

        result = "None";
    }
}
