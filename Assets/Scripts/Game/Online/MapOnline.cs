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

[System.Serializable]
public struct CellGameOnline
{
    [SyncVar] public GameObject co;
    public CellGameOnline(GameObject co)
    {
        this.co = co;
    }
}

[System.Serializable]
public class SyncListCell : SyncListStruct<CellGameOnline>
{ }

public class MapOnline : NetworkBehaviour
{
    [SyncVar] public LoadDataNet loadData;
    public Transform parent;
    public static MapOnline instance;
    [SyncVar] public float scale;
    public SyncListCell mapData;//convert то двуменый
    public GameObject[,] mapGo
    {
        get
        {
            GameObject[,] mapClone = new GameObject[loadData.x, loadData.y];
            int k = 0;
            for (int i = 0; i < loadData.x; i++)
                for (int j = 0; j < loadData.y; j++)
                {
                    mapClone[i, j] = mapData[k].co;
                    k++;
                }
           return mapClone;
        }
    }

    public override void OnStartServer()
    {
        if (isServer)
        {
            Debug.Log("StartServer");
            DataGame.LoadData();
            loadData.x = DataGame.x;
            loadData.y = DataGame.y;
            loadData.map = DataGame.str_map;
        }
    }
    private void Awake()
    {
        instance = this;
        parent = GameObject.Find("CanvasGame/ParentCell").transform;
    }

    [Command]
    public void CmdCreate()
    {
        List<List<string>> map = DataGame.StrToListMap(loadData.map, loadData.x, loadData.y);
        CellGame[,] mapSc = Map.CreateMap(loadData.x, loadData.y, 70, map, CustomNetManager.instance.CreateGOonlineCell);
        for (int i = 0; i < loadData.x; i++)
            for (int j = 0; j < loadData.y; j++)
                mapData.Add(new CellGameOnline(mapSc[i, j].gameObject));
    }

    SyncListCell GetServerData()
    {
        MapOnline[] ob = FindObjectsOfType<MapOnline>();
        foreach (MapOnline o in ob)
            if (o.mapData.Count > 1)
                return o.mapData;
        return null;
    }

    [Command]
    public void CmdGetMap()
    {
        RpcGetMap();
        RpcRefreshPos();
    }

    [ClientRpc]
    public void RpcGetMap()
    {
        Debug.Log("rpc get map");
        mapData = GetServerData();
    }


    [Command]
    public void CmdRefreshPos()
    {
        RpcRefreshPos();
    }

    [Command]
    public void CmdEnabled()
    {
        RpcEnabled();
    }

    [ClientRpc]
    public void RpcEnabled()
    {
        mapData[0].co.SetActive(!mapData[0].co.activeSelf);
    }
    [ClientRpc]
    public void RpcRefreshPos()
    {
        Debug.Log("update position all " + mapData.Count);
        Debug.Log(mapData[0]);
        Debug.Log(mapData[0].co);

            scale = Map.ScaleParent(parent, loadData.x, loadData.y);
            Map.RefreshPosition(mapGo, parent, loadData.x, loadData.y, 70);

    }

    public override void OnStartClient() { }
}

