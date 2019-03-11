using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SyncVar] public LoadDataNet loadData;
    [SyncVar] public float scale;

    public Transform parent;
    public SyncListCell mapCell;
    public CellGame[,] mapSc;

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
        List<List<string>> map = DataGame.StrToListMap(loadData.map, loadData.x, loadData.y);
        mapSc = Map.CreateMap(loadData.x, loadData.y, 70, map, CustomNetManager.instance.CreateGOonlineCell);

        for (int i = 0; i < loadData.x; i++)
            for (int j = 0; j < loadData.y; j++)
            {
                CellGam cell = new CellGam();
                cell.str = mapSc[i, j].cellData.str;
                mapCell.Add(cell);
            }
        scale = Map.ScaleParent(parent, loadData.x, loadData.y);
        Map.RefreshPosition(mapSc, parent, loadData.x, loadData.y, 70);
    }

    [Command]
    public void CmdGetMap()
    {
        NetworkConnection target = NetworkServer.connections[NetworkServer.connections.Count - 1];
        int k = 0;
        for (int i = 0; i < loadData.x; i++)
            for (int j = 0; j < loadData.y; j++)
            {
                TargetGetMap(target, mapSc[i, j].gameObject, i, j, k);
                k++;
            }
        TargetGetParent(target, parent.gameObject);
        TargetPosMap(target);
    }

    [TargetRpc]
    void TargetGetMap(NetworkConnection target, GameObject cell,int i, int j,int k)
    {
        if (i == 0 && j == 0) mapSc = new CellGame[loadData.x, loadData.y];
        CellGame cellGame = cell.AddComponent<CellGame>();
        cellGame.GetData(mapCell[k]);
        mapSc[i, j] = cellGame;
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
        Map.RefreshPosition(mapSc, parent, loadData.x, loadData.y, 70);
    }

    [ClientRpc]
    public void RpcEdit(int i, int j)
    {
        //mapClone[i,j].SetActive(!mapClone[i, j].activeSelf);
    }
}

