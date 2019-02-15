using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
    public Button btnStart;
    private void Start()
    {
        if (PlayerPrefs.GetString("map") != "")
            btnStart.interactable = true;
        else
        {
            DataGame.x = DataGame.y = 5;
        }
    }
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
