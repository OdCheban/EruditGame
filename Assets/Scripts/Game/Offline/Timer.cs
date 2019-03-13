using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {
    public float timer;
    public Text timeText;
    public bool pause;

    private void Update()
    {
        if (!pause)
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
}
