using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void StartEdit()
    {
        SceneManager.LoadScene("EditScene");
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
