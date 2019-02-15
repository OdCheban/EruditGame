using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EditScene : MonoBehaviour {

    public static EditScene instance;

    private EditScene()
    { }

    public static EditScene getInstance()
    {
        if (instance == null)
            instance = new EditScene();
        return instance;
    }

    public Text inputX;
    public Text inputY;
    private int x = 5 + 2;
    private int y = 5 + 2;
    public Vector2 startPos;
    public Transform cellBtn;
    public Transform parent;
    public List<GameObject> btnAbc;
    public List<Cell> cells;
    public Cell lastBtnCell;
    public Cell lastBtnAbc;

    public int kPlayers;

    public void EnterSize()
    {
        x = int.Parse(inputX.text) + 2;
        y = int.Parse(inputY.text) + 2;
        ScaleParent();
        ClearField();
        CreateField();
    }

    private void Awake()
    {
        instance = this;
        CreateField();
        AddListenerAbc();
        ScaleParent();
    }

    void ScaleParent()
    {
        float max = Mathf.Max(x, y);
        parent.localScale = new Vector2(9.5f / max, 9.5f / max);
    }

    private void AddListenerAbc()
    {
        foreach(GameObject go in btnAbc)
        {
            Cell cellAbc = go.AddComponent<Cell>();
            cellAbc.Create(Cell.TypeBtn.Abc);
            go.GetComponent<Button>().onClick.AddListener(()=> ClickCell(cellAbc, ref lastBtnAbc));
        }
    }

    private void ClearField()
    {
        cells.Clear();
        lastBtnCell = lastBtnAbc = null;
        foreach (Transform child in parent.transform)
        {
            Destroy(child.gameObject);
        }
        kPlayers = 0;   
    }

    private void CreateField()
    {
        Vector2 size = new Vector2(cellBtn.GetComponent<RectTransform>().rect.width, cellBtn.GetComponent<RectTransform>().rect.height);
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                if (DataGame.ExitRange(i, j,x,y))
                {
                    if ((j == y -1) || (j == 0) || (i== x-1) || (i == 0))
                        CreateBtn(startPos - new Vector2(size.x * i * -1, size.y * j), Cell.TypeBtn.Player);
                    else
                        CreateBtn(startPos - new Vector2(size.x * i * -1, size.y * j), Cell.TypeBtn.Default);
                }
            }
        }
    }

    void ClickCell(Cell cell, ref Cell lastBtn)
    {
        if(lastBtn != null)
        {
            lastBtn.UnClick();
        }
        if (lastBtn != cell)
        {
            lastBtn = cell;
            lastBtn.Click();
        }
        else
        {
            lastBtn = null;
        }
    }

    void CreateBtn(Vector2 pos,Cell.TypeBtn type)
    {
        GameObject btn = (GameObject)Instantiate(Resources.Load("CellBtn"), parent);
        Vector2 offset = new Vector2(-x * 42/2, y*45/2);
        btn.transform.localPosition = pos  + offset;
        Cell btnCell = btn.AddComponent<Cell>();
        btnCell.Create(type);
        btn.GetComponent<Button>().onClick.AddListener(() => ClickCell(btnCell,ref lastBtnCell));
        cells.Add(btnCell);
    }

    void SaveMap()
    {
        string m = "";
        foreach(Cell cell in cells)
        {
            if (cell.type == Cell.TypeBtn.Default)
            {
                switch(cell.txt.text)
                {
                    case "":
                        m += "cell ";
                        break;
                    case "wall":
                        m += "wall ";
                        break;
                    default:
                        m += cell.txt.text + " ";
                        break;
                }
            }
            if(cell.type == Cell.TypeBtn.Player)
            {
                switch (cell.txt.text)
                {
                    case "":
                        m += "null ";
                        break;
                    case "player":
                        m += "player ";
                        break;
                }
            }
        }
        PlayerPrefs.SetString("map", m);
        PlayerPrefs.SetInt("x", x);
        PlayerPrefs.SetInt("y", y);
        DataGame.x = x-2;
        DataGame.y = y-2;
    }

    public void Save()
    {
        SaveMap();
    }
    public void Menu()
    {
        SceneManager.LoadScene("Menu");
    }
    
}
