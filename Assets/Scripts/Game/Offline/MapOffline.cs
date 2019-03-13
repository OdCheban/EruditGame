using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapOffline : MonoBehaviour {
    public static MapOffline instance;

    public CellGame[,] mapCells = new CellGame[DataGame.x, DataGame.y];

    public Transform parent;
    [HideInInspector] public float scale;

    private void Awake()
    {
        instance = this;

        mapCells = Map.CreateMap(DataGame.x,DataGame.y,DataGame.sizeBtn,DataGame.map,Map.CreateGOcell);
        scale = Map.ScaleParent(parent, DataGame.x, DataGame.y);
        Map.StartPosition(mapCells, parent, DataGame.x, DataGame.y, DataGame.sizeBtn);

        StartCoroutine(CountdownToStart());
    }

    IEnumerator CountdownToStart()
    {
        yield return new WaitForSeconds(DataGame.timeExit);
        GetComponent<Timer>().enabled = true;
    }
}
