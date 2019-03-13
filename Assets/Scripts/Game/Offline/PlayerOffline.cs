using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player instance;

    public int N;//номер поезда

    public List<PlayerCell> playerCells;
    PlayerCell playerHead
    {
        get { return playerCells.Last(); }
    }
    public PlayerCell getPrev(PlayerCell cell)
    {
        int index = playerCells.IndexOf(cell) - 1;
        return (index >= 0)? playerCells[index]:null;
    }
    
    private Text textPoints;
    private GameObject arrow;

    public float speed
    {
        get
        {
            return (DataGame.sizeBtn) / (DataGame.speed + (playerCells.Count-1)*DataGame.speedVagon);
        }
    }
    public Vector2 velocity;
    public bool move;
    Color myColor;

    public bool connectIf;//возможность соед.
    private bool processConnect;

    private float _score;
    public float score
    {
        get { return _score; }
        set {
            _score = value;
            textPoints.text = _score.ToString();
        }
    }

    private void Awake()
    {
        textPoints = UIManager.instance.points_text;
        instance = this;
    }


    public void Create(int N, string typeStr, int i, int j, Color color)
    {
        this.N = N;
        CreateArrow();
        myColor = color;
        playerCells = new List<PlayerCell>();
        playerCells.Add(CreatePlayerCell(typeStr,i,j));


        transform.SetAsLastSibling();
    }

    void Update()
    {
        if (!processConnect)
            ControllPlayer();
        if (move)
            MovePlayer();

        playerHead.MoveNext(velocity);
        UIManager.instance.add_btn.interactable = connectIf = playerHead.CheckNextAbc();
    }

    string WordPlayer()
    {
        string m = "";
        for (int i = 1; i < playerCells.Count; i++)
            m += playerCells[i].cellData.str;
        return m.ToLower();
    }
    void RemovePlayerCells()
    {
        ChangeTargetArrow(playerCells[0].transform);
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
    void LetterExitCheck()
    {
        if (playerHead.ExitABC())
            foreach (string resultWords in DataGame.abcResult)
                if (resultWords == WordPlayer())
                {
                    for (int j = 1; j < playerCells.Count; j++)
                    {
                        score += DataGame.ABCscore[playerCells[j].cellData.str.ToLower()[0]];
                    }
                    score += WordPlayer().Length * DataGame.xBonus;
                    RemovePlayerCells();
                }
    }

    void MovePlayer()
    {
        foreach (PlayerCell player in playerCells)
            player.MoveToTarget(speed);
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
        UIManager.instance.rem_img.fillAmount = 0;
        while (UIManager.instance.rem_img.fillAmount < 1.0f)
        {
            UIManager.instance.rem_img.fillAmount += DataGame.speedDisconnect;
            yield return new WaitForEndOfFrame();
        }
        UIManager.instance.rem_btn.interactable = (playerCells.Count > 2) ? true : false;

        playerCells.Last().Disconnect();
        playerCells.Remove(playerCells.Last());
        ChangeTargetArrow(playerHead.transform);
        processConnect = false;
    }
    IEnumerator ConnectAnimation()
    {
        processConnect = true;
        UIManager.instance.add_img.fillAmount = 0;
        int nextI = playerHead.iTarget + (int)velocity.x;
        int nextJ = playerHead.jTarget + (int)velocity.y;
        MapOffline.instance.mapCells[nextI, nextJ].connectProcess = true;
        while (UIManager.instance.add_img.fillAmount < 1.0f)
        {
            UIManager.instance.add_img.fillAmount += DataGame.speedConnect;
            yield return new WaitForEndOfFrame();
        }
        playerCells.Add(playerHead.Connect(velocity));
        
        MapOffline.instance.mapCells[nextI, nextJ].connectProcess = false;
        UIManager.instance.rem_btn.interactable = (playerCells.Count > 1) ? true : false;
        ChangeTargetArrow(playerHead.transform);
        processConnect = false;
    }
     
    public PlayerCell CreatePlayerCell(string typeStr, int i, int j)
    {
        PlayerCell cell = gameObject.AddComponent<PlayerCell>();
        if (playerCells.Count == 0)
            cell = gameObject.AddComponent<PlayerCell>();
        else
        {
            GameObject newCell = Map.CreateGOcell(DataGame.sizeBtn);
            cell = newCell.AddComponent<PlayerCell>();
        }
        cell.Create(typeStr, i, j);
        cell.gameObject.GetComponent<Image>().color = myColor;
        return cell;
    }
    void EngineOff()
    {
        move = false;
        arrow.SetActive(false);
    }

    void CreateArrow()
    {
        arrow = (GameObject)Instantiate(Resources.Load("Arrow"), transform);
        arrow.GetComponent<RectTransform>().sizeDelta = new Vector2(DataGame.sizeBtn, DataGame.sizeBtn);
    }
    void ChangeTargetArrow(Transform target)
    {
        arrow.transform.SetParent(target);
        arrow.transform.localPosition = Vector3.zero;
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

    #region "Controll"
    void ControllPlayer()
    {
        if (Input.GetKeyDown(DataGame.key[0]) || Input.GetKeyDown(DataGame.key[1]) || Input.GetKeyDown(DataGame.key[2]) || Input.GetKeyDown(DataGame.key[3]))
        {
            if (Input.GetKeyDown(DataGame.key[0]))
            {
                if (velocity != new Vector2(1, 0))
                    velocity = new Vector2(-1, 0);
                else return;
            }
            if (Input.GetKeyDown(DataGame.key[1]))
            {
                if (velocity != new Vector2(-1, 0))
                    velocity = new Vector2(1, 0);
                else return;
            }
            if (Input.GetKeyDown(DataGame.key[2]))
            {
                if (velocity != new Vector2(0, 1))
                    velocity = new Vector2(0, -1);
                else return;
            }
            if (Input.GetKeyDown(DataGame.key[3]))
            {
                if (velocity != new Vector2(0, -1))
                    velocity = new Vector2(0, 1);
                else return;
            }
            if (!move)
                playerHead.NextCell(velocity);
            move = true;
        }
        if (Input.GetKeyDown(DataGame.key[6]))
        {
            EngineOff();
        }
        UpdateRotArrow();
        if (Input.GetKeyDown(DataGame.key[4]) && connectIf)
        {
            Connect();
        }
        if (Input.GetKeyDown(DataGame.key[5]))
        {
            Disconnect();
        }
    }
    #endregion
}