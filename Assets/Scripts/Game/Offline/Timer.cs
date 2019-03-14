using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Timer : NetworkBehaviour {
    public float timer;
    public Text timeText;
    public bool pause;

    public void StartTimer()
    {
        timer = DataGame.timeGame;
        pause = false;
    }

    private void Update()
    {
        if (!pause)
        {
            timer -= Time.deltaTime;
            CmdTimerRefresh("Time:" + timer.ToString("0"));
            if (timer <= 0)
            {
                UIManager.instance.CmdEndGame();
                enabled = false;
            }
        }
    }

    [Command]
    void CmdTimerRefresh(string m)
    {
        RpcTimerRefresh(m);
    }
    [ClientRpc]
    void RpcTimerRefresh(string m)
    {
        timeText.text = m;
    }
}
