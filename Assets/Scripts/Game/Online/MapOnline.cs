using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public struct LoadDataNet
{
    public float speed;
    public float speedVagon;
    public float speedDisconnect;
    public float speedConnect;
    public float timer;
    public int x;
    public int y;
    public string map;
}

[System.Serializable]
public struct CellOnline
{
    public int x;
    public int y;
    public string map;
}

[System.Serializable]
public struct CellGam
{
    public bool occup;
    public string str;
    public bool connectProcess;
}

public class SyncListCell: SyncListStruct<CellGam>
{
}

public class MapOnline : NetworkBehaviour
{
    public static MapOnline instance;
    [SyncVar] public LoadDataNet loadData;

    public Transform parent;
    public SyncListCell mapCells_sync;
    public CellGameOnline[,] mapCells;
    public List<PlayerOnline> players;

    private void Awake()
    {
        instance = this;
    }

    public override void OnStartServer()
    {
        if (isServer)
        {
            DataGame.LoadData();
            loadData.speed = DataGame.speed;
            loadData.timer = DataGame.timeGame;
            loadData.x = DataGame.maxI;
            loadData.y = DataGame.maxJ;
            loadData.map = DataGame.str_map;
            loadData.speedDisconnect = DataGame.speedDisconnect;
            loadData.speedConnect = DataGame.speedConnect;
            loadData.speedVagon = DataGame.speedVagon;
        }
    }

    [Command]
    public void CmdGetMap()
    {
        NetworkConnection target = NetworkServer.connections[NetworkServer.connections.Count - 1];
        for (int i = 0; i < loadData.x; i++)
            for (int j = 0; j < loadData.y; j++)
                TargetGetMap(target, mapCells[i,j].gameObject, (mapCells[i, j].upCell)?mapCells[i,j].upCell.gameObject:null,i,j);
    }

    [TargetRpc]
    public void TargetGetMap(NetworkConnection target, GameObject cell,GameObject upCell,int i,int j)
    {
        if(i+j == 0) mapCells = new CellGameOnline[loadData.x, loadData.y];
        mapCells[i, j] = cell.GetComponent<CellGameOnline>();
        if (upCell != null)
        {
            mapCells[i, j].upCell = upCell.GetComponent<CellUp>();
            mapCells[i, j].upCell.TextRefresh();
        }
    }

    [Command]
    public void CmdCreateMap()
    {
        CreateMap();
        StartPositionCell();

    }

    //создаем карту пустых клеток
    void CreateMap()
    {
        List<List<string>> mapStr = DataGame.StrToListMap(loadData.map, loadData.x, loadData.y);
        mapCells = new CellGameOnline[loadData.x,loadData.y];
        for (int i = 0; i < loadData.x; i++)
            for (int j = 0; j < loadData.y; j++)
            {
                mapCells[i, j] = CustomNetManager.instance.CreateGOonlineCell(mapStr[i][j]);
                mapCells[i, j].upCell = CustomNetManager.instance.CreateGOonlineCellUp(mapStr[i][j], i, j);
            }
    }
    
    void StartPositionCell()
    {
        float size = DataGame.sizeBtn;
        Vector2 offset = new Vector2(-loadData.x * size / 2, loadData.y * size / 1.7f);
        for (int i = 0; i < loadData.x; i++)
            for (int j = 0; j < loadData.y; j++)
            {
                mapCells[i, j].transform.position = offset + new Vector2((i + 0.5f) * (size + 0.2f), -(j + 0.5f) * (size + 0.2f));
                if (mapCells[i, j].upCell != null)
                    mapCells[i, j].upCell.transform.position = mapCells[i, j].transform.position;
            }
    }
}

