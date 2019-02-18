using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour {
    public static Main instance;

    public string[] resultWords;
    public CellGame[,] cells = new CellGame[DataGame.x, DataGame.y];
    public Transform parent;
    public Vector2 sizeBtn;

    public List<Player> players = new List<Player>();
    public Button[] btnsAdd;
    public Button[] btnsRem;
    public Text[] textsPoint;

    private void Awake()
    {
        instance = this;
        CreateMap();
    }

    private void CreateMap()
    {
        resultWords = PlayerPrefs.GetString("words").Split(' ', '\n');
        string m = PlayerPrefs.GetString("map");
        string[] map = m.Split(' ');
        ScaleParent();
        int q = 0;
        for (int i = 0; i < DataGame.x; i++)
            for (int j = 0; j < DataGame.y; j++)
            {
                if ((i != 0 || j != 0) &&
                   (i != DataGame.x - 1 || j != 0) &&
                   (i != 0 || j != DataGame.y - 1) &&
                   (i != DataGame.x - 1 || j != DataGame.y - 1))
                {
                    CreateCell(i, j, map[q]);
                    q++;
                }
            }

        for (int i = 0; i < players.Count; i++)
            players[i].gameObject.SetActive(false);
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(DataGame.timeExit);
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
            Vector2 offset = new Vector2(-DataGame.x * 60 / 2, DataGame.y * 60 / 2);
            btn.transform.localPosition = offset + new Vector2((i+0.5f) * sizeBtn.x, -(j+0.5f) * sizeBtn.y);
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
        cellGO.GetComponent<RectTransform>().sizeDelta = sizeBtn;
        cellGO.transform.localScale = Vector3.one;
        return cellGO;
    }
}
