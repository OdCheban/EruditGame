using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapOffline : MonoBehaviour {
    public static MapOffline instance;

    public CellGame[,] mapCells = new CellGame[DataGame.maxI, DataGame.maxJ];

    public Transform parent;

    private void Awake()
    {
        instance = this;

        mapCells = Map.CreateMap(DataGame.maxI,DataGame.maxJ,DataGame.sizeBtn,DataGame.map,Map.CreateGOcell);
        Map.StartPosition(mapCells, parent, DataGame.maxI, DataGame.maxJ, DataGame.sizeBtn);

        StartCoroutine(CountdownToStart());
    }

    IEnumerator CountdownToStart()
    {
        yield return new WaitForSeconds(DataGame.timeExit);
        GetComponent<Timer>().enabled = true;
    }
}
