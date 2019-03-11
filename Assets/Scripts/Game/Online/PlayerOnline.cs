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
            CmdEdit();
        }
    }

    [Command]
    public void CmdEdit()
    {
        MapOnline.instance.RpcEdit(2,3);
    }
}
