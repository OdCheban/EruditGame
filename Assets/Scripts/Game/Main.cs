using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour {
    public static Main instance;

    public CellGame[,] cells = new CellGame[DataGame.x, DataGame.y];
    public Transform parent;

    public List<Player> players = new List<Player>();
    public Button[] btnsAdd;
    public Button[] btnsRem;
    public Text[] textsPoint;
    public GameObject readyGame;

    private void Awake()
    {
        instance = this;
        CreateMap();
    }

    private void CreateMap()
    {
        ScaleParent();
        for (int i = 0; i < DataGame.x; i++)
        {
            int q = 0;
            for (int j = 0; j < DataGame.y; j++)
            {
                if ((i != 0 || j != 0) &&
                   (i != DataGame.x - 1 || j != 0) &&
                   (i != 0 || j != DataGame.y - 1) &&
                   (i != DataGame.x - 1 || j != DataGame.y - 1))
                {
                    CreateCell(i, j, DataGame.map[i][q]);
                    q++;
                }
            }
        }
        for (int i = 0; i < players.Count; i++)
            players[i].gameObject.SetActive(false);
        StartCoroutine(StartGame());
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

    void ScaleParent()
    {
        float max = Mathf.Max(DataGame.x, DataGame.y);
        parent.localScale = new Vector2(15.0f / max, 15.0f / max);
    }

    void CreateCell(int i, int j,string typeStr)
    {
        if (typeStr != "null")
        {
            GameObject btn = CreateGOcell();
            Vector2 offset = new Vector2(-DataGame.x * DataGame.sizeBtn  / 2, DataGame.y * DataGame.sizeBtn / 2);
            btn.transform.localPosition = offset + new Vector2((i+0.5f) * DataGame.sizeBtn, -(j+0.5f) * DataGame.sizeBtn);
            if (typeStr != "player")
            {
                cells[i, j] = btn.AddComponent<CellGame>();
                cells[i, j].Create(typeStr);
            }
            else
            {
                Player player = btn.AddComponent<Player>();
                player.Create(typeStr, i, j,DataGame.colorPlayers[players.Count]);
                players.Add(player);
            }
        }
    }

    public GameObject CreateGOcell()
    {
        GameObject cellGO = (GameObject)Instantiate(Resources.Load("Cell"), parent);
        cellGO.GetComponent<RectTransform>().sizeDelta = new Vector2(DataGame.sizeBtn, DataGame.sizeBtn);
        cellGO.transform.localScale = Vector3.one;
        return cellGO;
    }
}
