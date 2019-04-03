using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public struct LoadDataNet
{
    public float timeExit;
    public float speed;
    public float speedVagon;
    public float speedDisconnect;
    public float speedConnect;
    public float timer;
    public int x;
    public int y;
    public string map;
    public string abc;
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

    [NonSerialized] public List<string> abcList;
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
            loadData.timeExit = DataGame.timeExit;
            foreach (string d in DataGame.abcResult)
                loadData.abc += d + " ";
        }
    }

    [Command]
    public void CmdRoomInfo(int k, int maxK, string nameRoom)
    {
        RpcRoomInfo(k, maxK, nameRoom);
    }

    [ClientRpc]
    public void RpcRoomInfo(int k, int maxK, string nameRoom)
    {
        UIManager.instance.roomInfo.text = k + "/" + maxK + " " + nameRoom;
    }

    [ClientRpc]
    public void RpcStartRoom()
    {
        UIManager.instance.StartRoom();
    }
    [ClientRpc]
    public void RpcRoomChat(int k)
    {
        UIManager.instance.RoomChat(k);
    }
    [Command]
    public void CmdGetMap()
    {
        NetworkConnection target = NetworkServer.connections[NetworkServer.connections.Count - 1];
        TargetGetMap(target);
        abcList = loadData.abc.Split(' ').ToList();
    }

    [TargetRpc]
    public void TargetGetMap(NetworkConnection target)
    {
        int kChild = 0;
        mapCells = new CellGameOnline[loadData.x, loadData.y];
        for (int i = 0; i < loadData.x; i++)
        {
            for (int j = 0; j < loadData.y; j++)
            {
                GameObject child = parent.GetChild(kChild).gameObject;
                if(child.GetComponent<CellGameOnline>())
                {
                    kChild++;
                    mapCells[i, j] = child.GetComponent<CellGameOnline>();
                }
                if (kChild < parent.childCount)
                {
                    child = parent.GetChild(kChild).gameObject;
                    if (child.GetComponent<CellUp>())
                    {
                        kChild++;
                        mapCells[i, j].upCell = child.GetComponent<CellUp>();
                        mapCells[i, j].upCell.TextRefresh();
                    }
                }
            }
        }
        abcList = loadData.abc.Split(' ').ToList();
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
        {
            for (int j = 0; j < loadData.y; j++)
            {
                mapCells[i, j] = CustomNetManager.instance.CreateGOonlineCell();
                mapCells[i, j].upCell = CustomNetManager.instance.CreateGOonlineCellUp(mapStr[i][j], i, j);
            }
        }
    }
    
    void StartPositionCell()
    {
        float size = DataGame.sizeBtn;
        Vector2 offset = new Vector2(-loadData.x * size / 2, loadData.y * size / 1.7f);
        for (int i = 0; i < loadData.x; i++)
            for (int j = 0; j < loadData.y; j++)
            {
                mapCells[i, j].transform.position = offset + new Vector2((i + 0.5f) * (size + .05f), -(j + 0.5f) * (size + .05f));
                if (mapCells[i, j].upCell != null)
                    mapCells[i, j].upCell.transform.position = mapCells[i, j].transform.position;
            }
    }
}

