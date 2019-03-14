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

    public void EndGame()
    {
        Time.timeScale = 0;
        canvasEnd.SetActive(true);
        txtResult.text = "Игрок набрал " + Player.instance.score + " игрок!";
    }

    private string StrColor(Color color)
    {
        if (color == Color.red)
            return "<color=#ff0000ff>Красный</color>";
        if (color == Color.blue)
            return "<color=#0000ffff>Синий</color>";
        return "???";
    }
}
