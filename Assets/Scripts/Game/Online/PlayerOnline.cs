using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerOnline : NetworkBehaviour {
    private Text txtInfo;
    private void Update()
    {
        if (!isLocalPlayer) return;
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log(isLocalPlayer + " " + isServer + " " + isClient);
            MapOnline.instance.CmdEnabled();
        }
    }
}
