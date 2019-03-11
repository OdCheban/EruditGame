using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class CustomNetManager : NetworkManager {
    public static CustomNetManager instance;
    public MapOnline mapInstance;
    public bool create;
    public int kPlayers;

    private void Start()
    {
        instance = this;
    }

    public override void OnServerReady(NetworkConnection conn)
    {
    }

    IEnumerator UpdatePosPlayers()
    {
        while (!NetworkServer.connections[kPlayers-1].isReady)
            yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(1.0f);
        MapOnline.instance.CmdRefreshPos();
    }

    IEnumerator CreateMap()
    {
        while(!NetworkServer.connections[0].isReady)
        yield return new WaitForEndOfFrame();
        MapOnline.instance.CmdCreate();
        Debug.Log("map create");
        StartCoroutine(UpdatePosPlayers());
    }

    IEnumerator GetMapForUsers()
    {
        while (!NetworkServer.connections[kPlayers - 1].isReady)
            yield return new WaitForEndOfFrame();
        Debug.Log("map getter");
        MapOnline.instance.CmdGetMap();
        StartCoroutine(UpdatePosPlayers());
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        kPlayers = NetworkServer.connections.Count;
        GameObject player = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        GameObject mapGO = Instantiate(spawnPrefabs[1]);
        NetworkServer.SpawnWithClientAuthority(mapGO,player);

        Debug.Log("add player");
        if (!create)
        {
            StartCoroutine(CreateMap());
            create = true;
        }
        else
        {
            StartCoroutine(GetMapForUsers());
        }
    }

    public GameObject CreateGOonlineCell(int sizeBtn)
    {
        GameObject cellGO = Instantiate(spawnPrefabs[0]);
        NetworkServer.Spawn(cellGO);
        return cellGO;
    }
}
