using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[NetworkSettings(channel = 0, sendInterval = 0)]
public class CellGameOnline : NetworkBehaviour
{
    public CellUp upCell;
    [SyncVar] public bool connectProcess;
    void Awake()
    {
        GetComponent<RectTransform>().sizeDelta = new Vector2(DataGame.sizeBtn, DataGame.sizeBtn);
        transform.SetParent(MapOnline.instance.parent);
    }
    [ClientRpc]
    public void RpcOccup(NetworkInstanceId objId)
    {
        try
        {
            GameObject cell;
            if (isClient)
                cell = ClientScene.FindLocalObject(objId);
            else
                cell = NetworkServer.FindLocalObject(objId);
            if (!cell.GetComponent<CellUp>().destroy)
            {
                upCell = cell.GetComponent<CellUp>();
                GetComponent<Image>().color = Color.gray;
            }
        }catch
        {
            Debug.Log("errorOcuup");
        }
    }
    [ClientRpc]
    public void RpcLeave()
    {
        upCell = null;
        GetComponent<Image>().color = Color.white;
    }
}