using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ControllOnline : NetworkBehaviour {
    private Text txtInfo;
    private void Update()
    {
        if (!isLocalPlayer) return;
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CmdEdit();
        }
    }

    [Command]
    public void CmdEdit()
    {
        //Player.instance.
    }
}
