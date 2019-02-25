﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 1000, 20), Application.dataPath);
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
