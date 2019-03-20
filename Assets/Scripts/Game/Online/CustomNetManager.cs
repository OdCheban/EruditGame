using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class CustomNetManager : NetworkManager {
    public static CustomNetManager instance;
    public List<PlayerOnline> playerss = new List<PlayerOnline>();
    public List<NetworkConnection> netPlayers = new List<NetworkConnection>();
    public Timer timer;

    private void Start()
    {
        instance = this;
        UIManager.instance.gameObject.SetActive(true);
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        Debug.Log("add player");
        NetworkServer.AddPlayerForConnection(conn, playerss[NetworkServer.connections.Count - 1].gameObject, playerControllerId);
        netPlayers.Add(conn);
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);
        Debug.Log("ready");
        if (NetworkServer.connections.Count == 1)
            MapOnline.instance.CmdCreateMap();
        else
            MapOnline.instance.CmdGetMap();
        

        foreach (PlayerOnline player in playerss)
            player.RpcColorChange();

        UIManager.instance.RpcRoomInfo(netPlayers.Count, playerss.Count, UIManager.instance.nameConnRoom);

        if (NetworkServer.connections.Count == playerss.Count)
        {
            StartCoroutine(TimerToStartGame());
            foreach (PlayerOnline player in playerss)
                player.notStop = true;
        }
    }

    IEnumerator TimerToStartGame()
    {
        int k = 0;
        while (k < DataGame.timeExit)
        {
            UIManager.instance.RpcRoomChat((int)DataGame.timeExit - k);
            k++;
            yield return new WaitForSeconds(1.0f);
        }
        timer.StartTimer();
        UIManager.instance.RpcStartRoom();
    }

    public CellGameOnline CreateGOonlineCell(string m)
    {
        GameObject cellGO = Instantiate(spawnPrefabs[0]);
        NetworkServer.Spawn(cellGO);
        CellGameOnline cellSc = cellGO.GetComponent<CellGameOnline>();
        return cellSc;
    }

    public CellUp CreateGOonlineCellUp(string m, int i, int j)
    {
        CellUp cellUpsc = null;
        if (m != "cell")
        {
            GameObject cellUp = null;
            if (m == "player")
            {
                cellUp = Instantiate(spawnPrefabs[2]);
                PlayerOnline player = cellUp.GetComponent<PlayerOnline>();
                player.k = playerss.Count;
                playerss.Add(player);
            }
            else
            {
                cellUp = Instantiate(spawnPrefabs[1]);
            }
            NetworkServer.Spawn(cellUp);
            cellUpsc = cellUp.GetComponent<CellUp>();
            cellUpsc.CmdCreate(m, i, j);
        }
        return cellUpsc;
   }
   
}
