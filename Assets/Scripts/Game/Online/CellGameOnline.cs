using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


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
    public void RpcOccup(GameObject cell)
    {
        upCell = cell.GetComponent<CellUp>();
        GetComponent<Image>().color = Color.gray;
    }
    [ClientRpc]
    public void RpcLeave()
    {
        upCell = null;
        GetComponent<Image>().color = Color.white;
    }
}