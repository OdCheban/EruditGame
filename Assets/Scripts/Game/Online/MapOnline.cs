using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public struct LoadDataNet
{
    public int x;
    public int y;
    public string map;
}


public class MapOnline : NetworkBehaviour
{
    public static MapOnline instance;
    [SyncVar] public LoadDataNet loadData;
    [SyncVar] public float scale;

    public Transform parent;
    GameObject[,] mapClone;

    private void Awake()
    {
        instance = this;
    }

    public override void OnStartServer()
    {
        if (isServer)
        {
            DataGame.LoadData();
            loadData.x = DataGame.x;
            loadData.y = DataGame.y;
            loadData.map = DataGame.str_map;
        }
    }

    [Command]
    public void CmdCreateMap()
    {
        mapClone = new GameObject[loadData.x, loadData.y];
        List<List<string>> map = DataGame.StrToListMap(loadData.map, loadData.x, loadData.y);
        CellGame[,] mapSc = Map.CreateMap(loadData.x, loadData.y, 70, map, CustomNetManager.instance.CreateGOonlineCell);
        for (int i = 0; i < loadData.x; i++)
            for (int j = 0; j < loadData.y; j++)
                mapClone[i, j] = mapSc[i, j].gameObject;

        scale = Map.ScaleParent(parent, loadData.x, loadData.y);
        Map.RefreshPosition(mapClone, parent, loadData.x, loadData.y, 70);
    }

    [Command]
    public void CmdGetMap()
    {
        NetworkConnection target = NetworkServer.connections[NetworkServer.connections.Count - 1];
        for (int i = 0; i < loadData.x; i++)
            for (int j = 0; j < loadData.y; j++)
                TargetGetMap(target, mapClone[i, j], i, j);
        TargetGetParent(target, parent.gameObject);
        TargetPosMap(target);
    }

    [TargetRpc]
    void TargetGetMap(NetworkConnection target, GameObject cell,int i, int j)
    {
        if(i==0 && j ==0) mapClone = new GameObject[loadData.x, loadData.y];
        mapClone[i, j] = cell;
    }

    [TargetRpc]
    void TargetGetParent(NetworkConnection target, GameObject parent)
    {
        this.parent = parent.transform;
    }

    [TargetRpc]
    void TargetPosMap(NetworkConnection target)
    {
        parent.localScale = new Vector2(scale, scale);
        Map.RefreshPosition(mapClone, parent, loadData.x, loadData.y, 70);
    }

    [ClientRpc]
    public void RpcEdit(int i, int j)
    {
        mapClone[i,j].SetActive(!mapClone[i, j].activeSelf);
    }
}

