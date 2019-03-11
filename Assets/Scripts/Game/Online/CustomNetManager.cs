using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class CustomNetManager : NetworkManager {
    public static CustomNetManager instance;
    public MapOnline mapInstance;

    private void Start()
    {
        instance = this;
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject player = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);

        Debug.Log("add player");
        if (NetworkServer.connections.Count == 1)
        {
            MapOnline.instance.CmdCreateMap();
        }
        else
        {
            MapOnline.instance.CmdGetMap();
        }
    }

    public GameObject CreateGOonlineCell(int sizeBtn)
    {
        GameObject cellGO = Instantiate(spawnPrefabs[0]);
        cellGO.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeBtn, sizeBtn);
        NetworkServer.Spawn(cellGO);
        return cellGO;
    }
}
