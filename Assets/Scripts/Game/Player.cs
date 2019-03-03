using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    //состав поезда
    public List<PlayerCell> playerCells = new List<PlayerCell>();
    PlayerCell playerHead
    {
        get { return playerCells.Last(); }
    }
    public PlayerCell getPrev(PlayerCell cell)
    {
        int index = playerCells.IndexOf(cell) - 1;
        return (index >= 0)? playerCells[index]:null;
    }

    //системные характеристики
    KeyCode[] key;
    private Button btnAdd;
    private Button btnRem;
    private Text textPoints;
    private GameObject arrow;

    //характеристики поезда
    public enum MoveMode { Hard,Easy };
    public MoveMode moveMode;
    public float speed
    {
        get
        {
            return (DataGame.sizeBtn*Main.instance.scale) / (DataGame.speed + (playerCells.Count-1)*DataGame.speedVagon);
        }
    }
    public Vector2 velocity;
    public bool move;
    Color myColor;

    //игровые переменные
    public bool connectIf;//возможность соед.
    private bool processConnect;

    //статистика поезда
    private float _score;
    public float score
    {
        get { return _score; }
        set {
            _score = value;
            textPoints.text = _score.ToString();
        }
    }

    public void Create(string typeStr, int i, int j, Color color)
    {
        CreateArrow();
        myColor = color;
        playerCells.Add(CreateCell(typeStr,i,j));
        StartVelocity(i,j);
    }

    void CreateArrow()
    {
        arrow = (GameObject)Instantiate(Resources.Load("Arrow"), transform);
        arrow.GetComponent<RectTransform>().sizeDelta = new Vector2(DataGame.sizeBtn,DataGame.sizeBtn);
    }

    void UpdatePosArrow(Transform target)
    {
        arrow.transform.SetParent(target);
        arrow.transform.localPosition = Vector3.zero;
    }

    public void StartEngine(int moveMode, KeyCode[] key, Button btnAdd, Button btnRem, Text textPoints)
    {
        this.moveMode = (MoveMode)moveMode;
        textPoints.gameObject.SetActive(true);
        this.textPoints = textPoints;
        ButtonActivate(btnAdd, btnRem);
        this.key = key;

        transform.SetAsLastSibling();
    }
    string WordPlayer()
    {
        string m = "";
        for (int i = 1; i < playerCells.Count; i++)
            m += playerCells[i].str;
        return m.ToLower();
    }
    void RemovePlayerCells()
    {
        UpdatePosArrow(playerCells[0].transform);
        for (int i = 1; i < playerCells.Count; i++)
        {
            playerCells[i].DestroyCell();
            
        }
        int k = playerCells.Count;
        for (int i = 1; i < k; i++)
        {
            playerCells.RemoveAt(1);
        }
    }

    void Update()
    {
        if(!processConnect)
            Controll();
        if (move)
            MovePlayer();
        connectIf = playerHead.CheckArrive(velocity, moveMode);
        btnAdd.interactable = connectIf;

        if (playerHead.ExitABC())
            foreach (string resultWords in DataGame.abcResult)
                if (resultWords == WordPlayer())
                {
                    for (int j = 1; j < playerCells.Count; j++)
                    {
                        score += DataGame.ABCscore[playerCells[j].str.ToLower()[0]];
                    }
                    score += WordPlayer().Length * DataGame.xBonus;
                    RemovePlayerCells();
                }
    }

    void MovePlayer()
    {
        foreach (PlayerCell player in playerCells)
            player.MoveCell(speed);
    }

    public void Connect()
    {
        move = false;
        StartCoroutine(ConnectAnimation());
    }

    public void Disconnect()
    {
        if (playerCells.Count > 1)
        {
            StartCoroutine(DisConnectAnimation());
        }
    }
    IEnumerator DisConnectAnimation()
    {
        processConnect = true;
        btnRem.GetComponent<Image>().fillAmount = 0;
        while (btnRem.GetComponent<Image>().fillAmount < 1.0f)
        {
            btnRem.GetComponent<Image>().fillAmount += DataGame.speedDisconnect;
            yield return new WaitForEndOfFrame();
        }
        playerCells.Last().Disconnect();
        playerCells.Remove(playerCells.Last());
        btnRem.interactable = (playerCells.Count > 1) ? true : false;
        UpdatePosArrow(playerHead.transform);
        processConnect = false;
    }

    IEnumerator ConnectAnimation()
    {
        processConnect = true;
        btnAdd.GetComponent<Image>().fillAmount = 0;
        int nextI = playerHead.iTarget + (int)velocity.x;
        int nextJ = playerHead.jTarget + (int)velocity.y;
        Main.instance.cells[nextI, nextJ].connectProcess = true;
        while (btnAdd.GetComponent<Image>().fillAmount < 1.0f)
        {
            btnAdd.GetComponent<Image>().fillAmount += DataGame.speedConnect;
            yield return new WaitForEndOfFrame();
        }
        playerCells.Add(playerHead.Connect(velocity));
        Main.instance.cells[nextI, nextJ].connectProcess = false;
        btnRem.interactable = (playerCells.Count > 1) ? true : false;
        UpdatePosArrow(playerHead.transform);
        processConnect = false;
    }

    void ButtonActivate(Button btnAdd, Button btnRem)
    {
        btnAdd.gameObject.SetActive(true);
        btnRem.gameObject.SetActive(true);
        this.btnAdd = btnAdd;
        this.btnRem = btnRem;
        this.btnRem.onClick.RemoveAllListeners();
        this.btnAdd.onClick.RemoveAllListeners();
        this.btnRem.onClick.AddListener(() => Disconnect());
        this.btnAdd.onClick.AddListener(() => Connect());
    }

    void StartVelocity(int i, int j)
    {
        if (i == 0)
            velocity = new Vector2(1, 0);
        if (j == 0)
            velocity = new Vector2(0, 1);
        if (i == DataGame.x - 1)
            velocity = new Vector2(-1, 0);
        if (j == DataGame.y - 1)
            velocity = new Vector2(0, -1);
    }
     
    public PlayerCell CreateCell(string typeStr, int i, int j)
    {
        PlayerCell cell;
        if (playerCells.Count == 0)
            cell = gameObject.AddComponent<PlayerCell>();
        else
        {
            GameObject newCell = Main.instance.CreateGOcell();
            cell = newCell.AddComponent<PlayerCell>();
        }
        cell.Create(typeStr,i,j,this);
        cell.gameObject.GetComponent<Image>().color = myColor;
        return cell;
    }

    void Controll()
    {
        if (Input.GetKeyDown(key[0]) || Input.GetKeyDown(key[1]) || Input.GetKeyDown(key[2]) || Input.GetKeyDown(key[3]))
        {
            if (Input.GetKeyDown(key[0]))
            {
                if (velocity != new Vector2(1, 0))
                    velocity = new Vector2(-1, 0);
                else return;
            }
            if (Input.GetKeyDown(key[1]))
            {
                if (velocity != new Vector2(-1, 0))
                    velocity = new Vector2(1, 0);
                else return;
            }
            if (Input.GetKeyDown(key[2]))
            {
                if (velocity != new Vector2(0, 1))
                    velocity = new Vector2(0, -1);
                else return;
            }
            if (Input.GetKeyDown(key[3]))
            {
                if (velocity != new Vector2(0, -1))
                    velocity = new Vector2(0, 1);
                else return;
            }
            if(!move)
                playerHead.NextCell(velocity);
            move = true;
        }
        if (Input.GetKeyDown(key[6]))
        {
            EngineOff();
        }
        UpdateRotArrow();
        if (Input.GetKeyDown(key[4]) && connectIf)
        {
            Connect();
        }
        if (Input.GetKeyDown(key[5]))
        {
            Disconnect();
        }
    }

    void EngineOff()
    {
        move = false;
        arrow.SetActive(false);
    }
    void UpdateRotArrow()
    {
        if(velocity == new Vector2(0,-1))
            arrow.transform.rotation = Quaternion.Euler(Vector3.zero);
        if(velocity == new Vector2(0,1))
            arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0,-180));
        if (velocity == new Vector2(1, 0))
            arrow.transform.rotation = Quaternion.Euler(new Vector3(0,0, -90));
        if (velocity == new Vector2(-1, 0))
            arrow.transform.rotation = Quaternion.Euler(new Vector3(0,0, 90));
        if (move)
            arrow.SetActive(true);
    }
}
