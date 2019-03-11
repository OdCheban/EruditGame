using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapOffline : MonoBehaviour {
    public static MapOffline instance;

    public CellGame[,] cells = new CellGame[DataGame.x, DataGame.y];
    public Transform parent;

    public List<Player> players = new List<Player>();
    public Button[] btnsAdd;
    public Button[] btnsRem;
    public Text[] textsPoint;
    public GameObject readyGame;
    [HideInInspector] public float scale;
    private void Awake()
    {
        instance = this;
        Map.CreateMap(DataGame.x,DataGame.y,DataGame.sizeBtn,DataGame.map,Map.CreateGOcell);
        StartCoroutine(StartGame());
        for (int i = 0; i < players.Count; i++)
            players[i].gameObject.SetActive(false);
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(DataGame.timeExit);
        readyGame.SetActive(false);
        GetComponent<Timer>().enabled = true;
        for (int i = 0; i < players.Count; i++)
        {
            KeyCode[] control = new KeyCode[7] { DataGame.key[i, 0], DataGame.key[i, 1], DataGame.key[i, 2], DataGame.key[i, 3], DataGame.key[i, 4], DataGame.key[i, 5], DataGame.key[i, 6] };
            players[i].StartEngine(PlayerPrefs.GetInt("move"), control, btnsAdd[i], btnsRem[i], textsPoint[i]);
            players[i].gameObject.SetActive(true);
        }
    }
}
