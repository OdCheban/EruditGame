using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public struct LoadDataNet
{
    public float speedVagon;
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

public class SyncListCell: SyncListStruct<CellGam>
{
}

public class MapOnline : NetworkBehaviour
{
    public static MapOnline instance;
    public Timer timer;
    [SyncVar] public LoadDataNet loadData;
    [SyncVar] public float scale;

    public Transform parent;
    public SyncListCell mapCells_sync;
    public CellGame[,] mapCells;

    private void Awake()
    {
        instance = this;
    }

    public override void OnStartServer()
    {
        if (isServer)
        {
            DataGame.LoadData();
            loadData.speedVagon = DataGame.speedVagon;
            loadData.timer = DataGame.timeGame;
            loadData.x = DataGame.x;
            loadData.y = DataGame.y;
            loadData.map = DataGame.str_map;
        }
    }

    [Command]
    public void CmdCreateMap()
    {
        List<List<string>> map = DataGame.StrToListMap(loadData.map, loadData.x, loadData.y);
        mapCells = Map.CreateMap(loadData.x, loadData.y, 70, map, CustomNetManager.instance.CreateGOonlineCell);

        for (int i = 0; i < loadData.x; i++)
            for (int j = 0; j < loadData.y; j++)
            {
                CellGam cell = new CellGam();
                cell.str = mapCells[i, j].cellData.str;
                mapCells_sync.Add(cell);
            }
        scale = Map.ScaleParent(parent, loadData.x, loadData.y);
        Map.StartPosition(mapCells, parent, loadData.x, loadData.y, 70);
    }

    [Command]
    public void CmdGetMap()
    {
        NetworkConnection target = NetworkServer.connections[NetworkServer.connections.Count - 1];
        int k = 0;
        for (int i = 0; i < loadData.x; i++)
            for (int j = 0; j < loadData.y; j++)
            {
                TargetGetMap(target, mapCells[i, j].gameObject, i, j, k);
                k++;
            }
        TargetGetParent(target, parent.gameObject);
        TargetPosMap(target);
    }

    [TargetRpc]
    void TargetGetMap(NetworkConnection target, GameObject cell,int i, int j,int k)
    {
        if (i == 0 && j == 0) mapCells = new CellGame[loadData.x, loadData.y];
        CellGame cellGame = cell.AddComponent<CellGame>();
        cellGame.GetData(mapCells_sync[k]);
        mapCells[i, j] = cellGame;
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
        Map.StartPosition(mapCells, parent, loadData.x, loadData.y, 70);
    }

    [ClientRpc]
    public void RpcEdit(int i, int j)
    {
        if (!mapCells[i, j].cellData.occup)
            mapCells[i, j].Occup();
        else
            mapCells[i, j].Leave();
        //mapClone[i,j].SetActive(!mapClone[i, j].activeSelf);
    }


    public void StartGame()
    {
        timer.pause = true;
    }

}

