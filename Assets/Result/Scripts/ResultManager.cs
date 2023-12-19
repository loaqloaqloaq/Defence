using TMPro;
using UnityEngine;
using UnityEngine.Playables;

//-----------------------------------------
//���U���g�Ǘ�
//-----------------------------------------
public class ResultManager : MonoBehaviour
{
    //���� (YouWin or YouLose)
    [SerializeField] private TextMeshProUGUI resultText;

    //�G��|������ (��)
    //[SerializeField] private TextMeshProUGUI killText;
    //�N���A�^�C�� (��)
    //[SerializeField] private TextMeshProUGUI clearTimeText;
    [SerializeField] GameManager gameManager;
    void Start()
    {
        //���U���g��ʂ�Text���Z�b�g
        SetText();
    }

    //���U���g��ʂ�Text���Z�b�g
    private void SetText()
    {
        //���ݒ�
        if (gameManager.timer <= 0)
        { resultText.text = "youLose"; }
        else
        { resultText.text = "youWin"; }
    }
}
