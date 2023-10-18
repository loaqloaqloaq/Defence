using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using UnityEngine.Playables;

public class ResultScene : MonoBehaviour
{
    private Animator animator;

    [SerializeField] PlayableDirector director;

    [SerializeField] TextMeshProUGUI continueText;

    private float score;

    public enum textType
    {
        scoreText,
        killText,
        itemText,
        totalText,
        highScore,
        yourScoreText,
        Max
    }
    [SerializeField] private TextMeshProUGUI[] recordTexts;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void UpdateRecord(string type)
    {
        TextMeshProUGUI targetText = null;
        int value = 0;
        switch (type)
        {
            case "score":
                targetText = recordTexts[(int)textType.scoreText];
                value = Record.score;
                break;
            case "kill":
                targetText = recordTexts[(int)textType.killText];
                value = Record.killCount;
                break;
            case "item":
                targetText = recordTexts[(int)textType.itemText];
                value = Record.itemCount;
                break;
            case "total":
                targetText = recordTexts[(int)textType.totalText];
                value = Record.score + Record.killCount * 50 + Record.itemCount * 150;
                score = value;
                RenewHighScore(value);
                break;
            default:
                break;
        }
        if (targetText == null)
        {
            Debug.Log("text is null");
            return;
        }
        DataManager.Instance.LoadGameData();
        StartCoroutine(UpdateText(targetText, value));
    }

    //最高記録更新・上書き
    private static void RenewHighScore(int value)
    {
        int previousHighScore = (int)DataManager.Instance.data.data[(int)Data.dataType.highScore];  
        if (previousHighScore < value)
        {
            DataManager.Instance.data.SetHighScore(value);
            DataManager.Instance.SaveGameData();
        }
    }

    IEnumerator UpdateText(TextMeshProUGUI text, int end)
    {
        float grownRate = end * 0.01f;
        int current = 0;
        if (grownRate < 1)
        {
            grownRate = 1;
        }
        PlaySound("result_Sfx1");
        while (current < end)
        {
            current += (int)grownRate;
            if (current > end)
            {
                current = end;
            }
            text.text = current.ToString();
            yield return new WaitForSeconds(0.008f);
        }
    }

    //入力可能にする
    public void ActivateInput()
    {
        StartCoroutine(WaitForInput());
    }

    //次の処理に移るまでキー入力を待つ
    IEnumerator WaitForInput()
    {
        continueText.gameObject.SetActive(true);
        float time = 0;
        while (true)
        {
            float alpha;
            time += Time.deltaTime;
            alpha = Mathf.Abs(Mathf.Sin(time));
            continueText.color = new Color(1, 1, 1, alpha);
            if (Input.anyKey)
            {
                PlaySound("result_Sfx1");
                continueText.gameObject.SetActive(false);
                ToNext();
                break;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public void ReturnToTitle()
    {
        SceneManager.LoadScene("Title");
    }

    public void ReturnToGame()
    {
        SceneManager.LoadScene("GameScene");
    }
   
    //UIアニメーションからの呼出
    public void ToNext()
    {
        animator.Play("toNext");
        var data = DataManager.Instance.data;
        recordTexts[(int)textType.highScore].text = ((int)data.data[(int)Data.dataType.highScore]).ToString();
        recordTexts[(int)textType.yourScoreText].text = score.ToString();
    }

    public void PlaySound(string soundName)
    {
        SoundManager.Instance.Play("Sounds/UI_Sfx/" + soundName, SoundManager.Sound.UI);
    }
}
