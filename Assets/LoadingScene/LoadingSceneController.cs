using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingSceneController : MonoBehaviour
{
    static string nextScene;

    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshProUGUI guideText;

    private AsyncOperation op;

    private bool InputActivation;

    public static void RestartScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadSceneProcess());
    }

    private IEnumerator LoadSceneProcess()
    {
        Time.timeScale = 1f;
        op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;
        InputActivation = false;
        
        float timer = 0f;
        while (!op.isDone)
        {
            yield return null;
            if (op.progress < 0.7f)
            {
                progressBar.fillAmount = op.progress;
            }
            else
            {
                timer += Time.deltaTime;
                Debug.Log(timer);
                progressBar.fillAmount = Mathf.Lerp(0.7f, 1f, timer);
                if (progressBar.fillAmount >= 1f)
                {
                    InputActivation = true;
                    guideText.text = "PRESS ANY KEY TO START";
                    yield break;
                }
                yield return new WaitForSeconds(Time.deltaTime);
            }
            guideText.text = (progressBar.fillAmount * 100).ToString("00") + "%";
        }
    }

    private void Update()
    {
        if (InputActivation && Input.anyKeyDown) 
        {
            op.allowSceneActivation = true;
        }
    }
}