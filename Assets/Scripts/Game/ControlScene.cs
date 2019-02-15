using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControlScene : MonoBehaviour {
    public static ControlScene instance;
    public GameObject canvasEnd;
    public Text txtResult;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("Menu");
    }
    public void EndGame()
    {
        Time.timeScale = 0;
        string strResult = "";
        List<Player> winPlayers = new List<Player>();
        foreach (Player player in Main.instance.players)
        {
            if (winPlayers.Count == 0 || player.score > winPlayers[0].score)
            {
                winPlayers.Clear();
                winPlayers.Add(player);
            }else if(player.score == winPlayers[0].score)
            {
                winPlayers.Add(player);
            }
            strResult += StrColor(player.GetComponent<Image>().color) + " игрок получил " + player.score + " очков" + "\n";
        }
        strResult += "\n";

        string nameWiners = "";
        foreach (Player winPlayer in winPlayers)
            nameWiners += StrColor(winPlayer.GetComponent<Image>().color) + " ";

        canvasEnd.SetActive(true);
        txtResult.text = strResult + "Победил " + nameWiners + " игрок!";
    }

    private string StrColor(Color color)
    {
        if (color == Color.red)
            return "<color=#ff0000ff>Красный</color>";
        if (color == Color.blue)
            return "<color=#0000ffff>Синий</color>";
        return "???";
    }

    public void MenuBtn()
    {
        SceneManager.LoadScene("Menu");
    }
}
