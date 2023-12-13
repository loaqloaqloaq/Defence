using TMPro;
using UnityEngine;
using UnityEngine.Playables;

//-----------------------------------------
//リザルト管理
//-----------------------------------------
public class ResultManager : MonoBehaviour
{
    //結果 (YouWin or YouLose)
    [SerializeField] private TextMeshProUGUI resultText;

    //敵を倒した数 (仮)
    //[SerializeField] private TextMeshProUGUI killText;
    //クリアタイム (仮)
    //[SerializeField] private TextMeshProUGUI clearTimeText;
    [SerializeField] GameManager gameManager;
    void Start()
    {
        //リザルト画面のTextをセット
        SetText();
    }

    //リザルト画面のTextをセット
    private void SetText()
    {
        //仮設定
        if (gameManager.timer <= 0)
        { resultText.text = "youLose"; }
        else
        { resultText.text = "youWin"; }
    }
}
