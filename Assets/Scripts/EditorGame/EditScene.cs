using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;

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

    public InputField inputX;
    public InputField inputY;
    public InputField inputWordResult;
    public InputField inputWordResultN;
    public InputField timeExit;
    public InputField timeGame;
    public InputField speed;
    public InputField speedVagon;
    public InputField speedConnect;
    public InputField speedDisconnect;
    public InputField xBonus;

    public Slider moveMode;
    private int x;
    private int y;
    public Vector2 startPos;
    public Transform cellBtn;
    public Transform parent;
    public List<GameObject> btnAbc;
    public List<Cell> cells;
    public Cell lastBtnCell;
    public Cell lastBtnAbc;

    public int kPlayers;

    int n;
    string s;//исходное слово
    List<int> a = new List<int>();
    List<string> mas = new List<string>();

    //функция генерации подмножеств
    string next_pdm()
    {
        //string s1;
        StringBuilder s1;
        int i;
        i = n;
        while (a[i] == 1)
        {
            a[i] = 0;
            i--;
        }
        a[i]++;
        s1 = new StringBuilder();
        for (i = 1; i <= n; i++)
            if (a[i] == 1)
                s1.Append(s[i-1]);
        return s1.ToString();
    }

    //генерация перестановок
    void next_per(ref string s2, int l, int r, ref int m)
    {
        StringBuilder s1 = new StringBuilder(s2);
        int i;
        char v;
        if (r > 5) return;
        if (l == r)
        {
            m++;
            mas.Add(s1.ToString());
        }
        else
        {
            for (i = l; i <= r; i++)
            {
                v = s1[l];
                s1[l] = s1[i-1];
                s1[i-1] = v; //обмен s1[i],s1[l]
                s2 = s1.ToString();
                if (l + 1 > 5) return;
                next_per(ref s2, l + 1, r, ref m); //вызов новой генерации
                v = s1[l];
                s1[l] = s1[i-1];
                s1[i-1] = v; //обмен s1[i],s1[l]
            }
        }
    }
        
    
    List<string> mass = new List<string>();
    //печать массива
    void Print(int m)
    {
        for (int i = 0; i < m; i++)
        {
            if(mas[i].Length > 1)
            mass.Add(mas[i]);
        }
    }

    void Hi(string ss)
    {
        mass.Clear();
        a.Clear();
        mas.Clear();
        string s1;
        int m = 0;
        s = ss;
        n = ss.Length;
        a.Add(0);
        for (int i = 1; i <= n; i++)
            a.Add(0);
        a[n] = 1;
        while (a[0] == 0)
        {
            s1 = next_pdm();
            next_per(ref s1, 2, s1.Length, ref m);
        }
        Print(m);
    }

    string StringCell()
    {
        string m = string.Empty;
        foreach (Cell cell in cells)
            if (cell.type == Cell.TypeBtn.Default && cell.text != "")
                m += cell.text;
        return m;
    }


    public void CheckAvialWords()
    {
        Hi(StringCell().ToLower());
        HashSet<string> res = new HashSet<string>();
        int k = 0;
        for (int i = 0; i < mass.Count; i++)
            foreach (string r in DataGame.allWords[mass[i][0]][mass[i][1]])
            {
                if (r == mass[i])
                    res.Add(mass[i]);
                k++;
            }
        inputWordResultN.text = "N = " + res.Count;
        inputWordResult.text = "";
        foreach (string t in res)
            inputWordResult.text += t + " ";
    }

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
        LoadFieldSettings();
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
                    bool whoIs = ((j == y - 1) || (j == 0) || (i == x - 1) || (i == 0));
                    Cell.TypeBtn who = (whoIs) ? Cell.TypeBtn.Player : Cell.TypeBtn.Default;
                    CreateBtn(startPos - new Vector2(size.x * i * -1, size.y * j), who);
                }
            }
        }
    }


    private void LoadFieldSettings()
    {
        x = DataGame.x;
        y = DataGame.y;
        ScaleParent();
        LoadSettings();
        LoadField();
    }
    private void LoadSettings()
    {
        inputX.text = DataGame.x.ToString();
        inputY.text = DataGame.y.ToString();
        inputWordResult.text = string.Join(" ", DataGame.abcResult.ToArray());
        timeExit.text = DataGame.timeExit.ToString();
        timeGame.text = DataGame.timeGame.ToString();
        speed.text = DataGame.speed.ToString();
        speedVagon.text = DataGame.speedVagon.ToString();
        speedConnect.text = DataGame.speedConnect.ToString();
        speedDisconnect.text = DataGame.speedDisconnect.ToString();
        xBonus.text = DataGame.xBonus.ToString();
        moveMode.value = DataGame.modeMove;
    }
    private void LoadField()
    {
        Vector2 size = new Vector2(cellBtn.GetComponent<RectTransform>().rect.width, cellBtn.GetComponent<RectTransform>().rect.height);
        for (int i = 0; i < DataGame.x; i++)
        {
            int iY = 0; 
            for (int j = 0; j < DataGame.y; j++)
            {
                if (DataGame.ExitRange(i, j, x, y))
                {
                    bool whoIs = ((j == y - 1) || (j == 0) || (i == x - 1) || (i == 0));
                    Cell.TypeBtn who = (whoIs) ? Cell.TypeBtn.Player : Cell.TypeBtn.Default;
                    CreateBtn(startPos - new Vector2(size.x * i * -1, size.y * j), who,DataGame.map[i][iY]);
                    iY++;
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
    void CreateBtn(Vector2 pos,Cell.TypeBtn type,string txt = "")
    {
        GameObject btn = (GameObject)Instantiate(Resources.Load("CellBtn"), parent);
        Vector2 offset = new Vector2(-x * 42/2, y*45/2);
        btn.transform.localPosition = pos  + offset;
        Cell btnCell = btn.AddComponent<Cell>();
        btnCell.Create(type);
        btn.GetComponent<Button>().onClick.AddListener(() => ClickCell(btnCell,ref lastBtnCell));
        cells.Add(btnCell);

        if(txt != "cell" && txt != "null")
        btnCell.txt.text = txt;
    }
    void SaveMap()
    {
        PlayerPrefs.SetString("words", inputWordResult.text);
        PlayerPrefs.SetFloat("timeExit", float.Parse(timeExit.text));
        PlayerPrefs.SetFloat("timeGame", float.Parse(timeGame.text));
        PlayerPrefs.SetFloat("speed", float.Parse(speed.text));
        PlayerPrefs.SetFloat("speedVagon", float.Parse(speedVagon.text));
        PlayerPrefs.SetFloat("speedConnect", float.Parse(speedConnect.text));
        PlayerPrefs.SetFloat("speedDisconnect", float.Parse(speedDisconnect.text));
        PlayerPrefs.SetFloat("xBonus", float.Parse(xBonus.text));
        PlayerPrefs.SetInt("x", x-2);
        PlayerPrefs.SetInt("y", y-2);
        PlayerPrefs.SetInt("move", (int)moveMode.value);
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
    }
    public void Save()
    {
        SaveMap();
        DataGame.LoadData();
    }
    public void Menu()
    {
        SceneManager.LoadScene("Menu");
    } 
}
