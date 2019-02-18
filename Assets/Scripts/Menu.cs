using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
    public Button btnStart;
    private void Start()
    {
        if (PlayerPrefs.GetString("map") != "" || PlayerPrefs.GetInt("timeExit",-1) != -1)
            btnStart.interactable = true;
        else
        {
            DataGame.x = DataGame.y = 5;
            DataGame.move = PlayerPrefs.GetInt("move");
            DataGame.timeExit = PlayerPrefs.GetInt("timeExit");
            DataGame.timeGame = PlayerPrefs.GetInt("timeGame");
            DataGame.speed = PlayerPrefs.GetInt("speed");
            DataGame.speedVagon = PlayerPrefs.GetInt("speedVagon");
            DataGame.speedConnect = PlayerPrefs.GetInt("speedConnect");
            DataGame.speedDisconnect = PlayerPrefs.GetInt("speedDisconnect");
            DataGame.xBonus = PlayerPrefs.GetInt("xBonus");
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
