using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class CustomNetManager : NetworkManager {
    public static CustomNetManager instance;
    public List<PlayerOnline> players = new List<PlayerOnline>();
    public List<NetworkConnection> netPlayers = new List<NetworkConnection>();
    public Timer timer;

    private void Start()
    {
        instance = this;
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        if (NetworkServer.connections.Count == 1)
            MapOnline.instance.CmdCreateMap();
        else
            MapOnline.instance.CmdGetMap();

        foreach (PlayerOnline player in players)
        {
            if (player.k == NetworkServer.connections.Count - 1)
            {
                NetworkServer.AddPlayerForConnection(conn, player.gameObject, playerControllerId);
                netPlayers.Add(conn);
            }
            player.RpcColorChange();
        }
        UIManager.instance.RpcRoomInfo(netPlayers.Count, players.Count);

        if (NetworkServer.connections.Count == players.Count)
        {
            StartCoroutine(TimerToStartGame());
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
                player.k = players.Count;
                players.Add(player);
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
