using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {
    [SerializeField]private float timer;
    public Text timeText;
    private void Start()
    {
        timer = DataGame.timeGame;
    }
    private void Update()
    {
        timer -= Time.deltaTime;
        timeText.text = "Time:" + timer.ToString("0");
        if (timer <= 0)
        {
            ControlScene.instance.EndGame();
            enabled = false;
        }
    }
}
