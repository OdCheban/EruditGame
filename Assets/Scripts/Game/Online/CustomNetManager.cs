using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomNetManager : NetworkManager {
    public static CustomNetManager instance;
    public UIManager uimanager;
    public List<PlayerOnline> playerss = new List<PlayerOnline>();
    public List<NetworkConnection> netPlayers = new List<NetworkConnection>();
    public Timer timer;
    public bool readyLastPlayers;

    private void Start()
    {
        instance = this;
        if (matchMaker == null)
            StartMatchMaker();
        uimanager.RefreshRoom();
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        if (NetworkServer.connections.Count == 1)
            MapOnline.instance.CmdCreateMap();
        NetworkServer.AddPlayerForConnection(conn, playerss[NetworkServer.connections.Count - 1].gameObject, playerControllerId);
        netPlayers.Add(conn);
        readyLastPlayers = true;
    }



    public override void OnServerReady(NetworkConnection conn)
    {
        readyLastPlayers = false;

        if (NetworkServer.connections.Count > playerss.Count+1)
        {
            conn.Disconnect();
            DebugUI.instance.SetText("disconnect users by range");
        }
        StartCoroutine(WaitReadyLastPlayers());
        base.OnServerReady(conn);
    }

    IEnumerator WaitReadyLastPlayers()
    {
        while (!readyLastPlayers)
            yield return new WaitForSeconds(1.0f);
        Debug.Log("readyPlayer");
        if (NetworkServer.connections.Count > 1)
            MapOnline.instance.CmdGetMap();
        foreach (PlayerOnline player in playerss)
            player.RpcColorChange();

        MapOnline.instance.CmdRoomInfo(netPlayers.Count, playerss.Count, uimanager.nameConnRoom);//error

        if (NetworkServer.connections.Count == playerss.Count)
            StartCoroutine(TimerToStartGame());
    }

    IEnumerator TimerToStartGame()
    {
        int k = 0;
        while (k < DataGame.timeExit)
        {
            MapOnline.instance.RpcRoomChat((int)DataGame.timeExit - k);
            k++;
            yield return new WaitForSeconds(1.0f);
        }
        foreach (PlayerOnline player in playerss)
            player.notStop = true;
        timer.StartTimer();
        MapOnline.instance.RpcStartRoom();
    }

    public CellGameOnline CreateGOonlineCell()
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
