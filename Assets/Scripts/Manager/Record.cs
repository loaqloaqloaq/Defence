static public class Record 
{
    //プレイ中に点数とかかわるすべてをこのクラスに追加
    static public int score;
    static public int killCount;
    static public int itemCount;
    static public int resultID; //結果 (1:自分の拠点を守り切ってクリア 2:敵拠点を壊し切ってクリア 3:ライフがなくなって負けた or 敵拠点を全て壊された)

    static public void Init()
    {
        score = 0;
        killCount = 0;
        itemCount = 0;
        resultID = 0;
    }
}
