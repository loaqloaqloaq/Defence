using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    public void StartGameButtonDown()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ExitButtonDown()
    {
        Application.Quit();
    }
}
