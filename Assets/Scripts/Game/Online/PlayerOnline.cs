using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[NetworkSettings(channel = 0, sendInterval = 0.01f)]
public class PlayerOnline : CellUp
{
    public static PlayerOnline instance;
    public GameObject arrow;

    public bool connectIf;//возможность соед.
    [SyncVar] public bool move;
    [SyncVar] public bool notStop;
    [SyncVar] public Vector2 velocity;
    [SyncVar] public bool processConnect;
    [SyncVar] public int k;

    public List<CellUp> playerCells;
    private float _score;
    public float score
    {
        get { return _score; }
        set
        {
            _score = value;
            UIManager.instance.points_text.text = _score.ToString();
        }
    }
    CellUp playerHead
    {
        get { return playerCells.Last(); }
    }
    public CellUp getPrev(CellUp cell)
    {
        int index = playerCells.IndexOf(cell) - 1;
        return (index >= 0) ? playerCells[index] : null;
    }
    public float speed
    {
        get
        {
            LoadDataNet load =  MapOnline.instance.loadData;
            return (DataGame.sizeBtn) / (load.speed + (playerCells.Count - 1) * load.speedVagon);
        }
    }
    protected override void AwakeExtra()
    {
        arrow = transform.Find("Arrow").gameObject;
        arrow.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(DataGame.sizeBtn, DataGame.sizeBtn);
        arrow.SetActive(false);
        playerCells.Add(this);
    }
    private void Start()
    {
        if (isLocalPlayer) instance = this;
    }
    
    [Command]
    public void CmdUnConnectEnd(GameObject target, int i, int j)
    {
        RpcUnConnectEnd(target, i, j);
    }
    [ClientRpc]
    public void RpcUnConnectEnd(GameObject target, int i, int j)
    {
        MapOnline.instance.mapCells[i, j].upCell = target.GetComponent<CellUp>();
        MapOnline.instance.mapCells[i, j].upCell.unConnect = false;
    }

    [Command]
    public void CmdNext(GameObject target, int i, int j)
    {
        CellUp cellUp = target.GetComponent<CellUp>();
        cellUp.nextI = i;
        cellUp.nextJ = j;
    }
    [Command]
    public void CmdPos(GameObject target, int ii, int jj)
    {
        CellUp cellTarget = target.GetComponent<CellUp>();
        cellTarget.iPos = ii;
        cellTarget.jPos = jj;
    }
    [Command]
    public void CmdTarget(GameObject target, int i, int j)
    {
        CellUp cellUp = target.GetComponent<CellUp>();
        cellUp.iTarget = i;
        cellUp.jTarget = j;
    }

    [Command]
    public void CmdProcessConnect(bool flag)
    {
        processConnect = flag;
    }

    [Command]
    public void CmdDisconnect()
    {
        playerCells.Last().RpcDisconnectCell();
        RpcDisconnect();
        RpcChangeTargetArrow();
    }
    [ClientRpc]
    public void RpcDisconnect()
    {
        playerCells.Remove(playerCells.Last());
    }
    [Command]
    public void CmdConnect()
    {
        GameObject newVagon = playerHead.Connect(velocity).gameObject;
        RpcConnect(newVagon);
        RpcChangeTargetArrow();
    }
    [ClientRpc]
    public void RpcConnect(GameObject obj)
    {
        playerCells.Add(obj.GetComponent<CellUp>());
        obj.transform.SetAsLastSibling();
        playerHead.GetComponent<Image>().color = icon.color;
    }
    [ClientRpc]
    public void RpcChangeTargetArrow()
    {
        arrow.transform.SetParent(playerHead.gameObject.transform);
        arrow.transform.localPosition = Vector3.zero;
    }
    [ClientRpc]
    public void RpcColorChange()
    {
        icon.color = DataGame.colorPlayers[k];
        transform.SetAsLastSibling();
    }
    [Command]
    void CmdVel(Vector2 vel)
    {
        velocity = vel;
    }

    [Command]
    void CmdConnectCell(bool flag)
    {
        MapOnline.instance.mapCells[nextI,nextJ].connectProcess = flag;
    }

    [Command]
    public void CmdMove(bool move)
    {
        this.move = move;
    }

    private void Update()
    {
        if (isLocalPlayer && notStop)
        {
            if(!processConnect)
                Controll();
            if (move)
            {
                playerHead.MoveNext(velocity);
                CmdMovePlayer();
            }
            UIManager.instance.add_btn.interactable = connectIf = playerHead.CheckNextAbc();
            LetterExitCheck();
        }
        UpdateRotArrow();
    }
    string WordPlayer()
    {
        string m = "";
        for (int i = 1; i < playerCells.Count; i++)
            m += playerCells[i].str;
        return m.ToLower();
    }

    void LetterExitCheck()
    {
        if (playerHead.ExitABC())
            foreach (string resultWords in MapOnline.instance.abcList)
                if (resultWords == WordPlayer())
                {
                    for (int j = 1; j < playerCells.Count; j++)
                    {
                        score += DataGame.ABCscore[playerCells[j].str.ToLower()[0]];
                    }
                    score += WordPlayer().Length * DataGame.xBonus;
                    CmdRemovePlayerCells();
                }
    }

    [Command]
    void CmdRemovePlayerCells()
    {
        RpcRemoveCells();
        RpcChangeTargetArrow();
    }

    [ClientRpc]
    void RpcRemoveCells()
    {
        try
        {
            List<CellUp> destrCell = new List<CellUp>(playerCells);
            int k = playerCells.Count;
            for (int i = 1; i < k; i++)
            {
                playerCells.RemoveAt(1);
            }
            
            for (int i = 1; i < destrCell.Count; i++)
            {
                destrCell[i].RpcDestroyCell();
            }
        }catch
        {
            Debug.Log("error");
        }
    }


    [Command]
    void CmdMovePlayer()
    {
        foreach (CellUp player in playerCells)
            player.RpcMoveToTarget(speed);
    }
    void Controll()
    {
        if (Input.GetKeyDown(DataGame.key[0]) || Input.GetKeyDown(DataGame.key[1]) || Input.GetKeyDown(DataGame.key[2]) || Input.GetKeyDown(DataGame.key[3]))
        {
            if (Input.GetKeyDown(DataGame.key[1]))
            {
                if (velocity != new Vector2(1, 0))
                    CmdVel(new Vector2(-1, 0));
                else return;
            }
            if (Input.GetKeyDown(DataGame.key[0]))
            {
                if (velocity != new Vector2(-1, 0))
                    CmdVel(new Vector2(1, 0));
                else return;
            }
            if (Input.GetKeyDown(DataGame.key[3]))
            {
                if (velocity != new Vector2(0, 1))
                    CmdVel(new Vector2(0,-1));
                else return;
            }
            if (Input.GetKeyDown(DataGame.key[2]))
            {
                if (velocity != new Vector2(0, -1))
                    CmdVel(new Vector2(0, 1));
                else return;
            }
            CmdMove(true);
        }
        if (Input.GetKeyDown(DataGame.key[4]) && connectIf)
        {
            Connect();
        }
        if (Input.GetKeyDown(DataGame.key[5]))
        {
            Disconnect();
        }
        //if (Input.GetKeyDown(DataGame.key[6]))
        //    CmdEngineOff();
    }

    public void Connect()
    {
        CmdMove(false);
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
        CmdProcessConnect(true);
        UIManager.instance.rem_img.fillAmount = 0;
        while (UIManager.instance.rem_img.fillAmount < 1.0f)
        {
            UIManager.instance.rem_img.fillAmount += MapOnline.instance.loadData.speedDisconnect;
            yield return new WaitForEndOfFrame();
        }
        UIManager.instance.rem_btn.interactable = (playerCells.Count > 2) ? true : false;
        CmdDisconnect();
        CmdProcessConnect(false);
    }
    IEnumerator ConnectAnimation()
    {
        CmdProcessConnect(true);
        UIManager.instance.add_img.fillAmount = 0;
        CmdConnectCell(true);
        while (UIManager.instance.add_img.fillAmount < 1.0f)
        {
            UIManager.instance.add_img.fillAmount += MapOnline.instance.loadData.speedConnect;
            yield return new WaitForEndOfFrame();
        }
        CmdConnect();
        CmdConnectCell(false);
        UIManager.instance.rem_btn.interactable = true;
        CmdProcessConnect(false);
    }

    [Command]
    void CmdEngineOff()
    {
        CmdMove(false);
    }

    [Command]
    public void CmdOccupTo(GameObject obj, int i, int j)
    {
        MapOnline.instance.mapCells[i, j].RpcOccup(obj);
    }
    [Command]
    public void CmdLeaveTo(GameObject obj, int i, int j)
    {
        CellUp prev = getPrev(obj.GetComponent<CellUp>());
        if (prev != null)
        {
            CmdLeaveTo(prev.gameObject, prev.iTarget, prev.jTarget);
        }
        LoadDataNet ldn = MapOnline.instance.loadData;
        if (DataGame.ExitRangeGame(i, j, ldn.x, ldn.y))
            MapOnline.instance.mapCells[i, j].RpcLeave();
    }

    void UpdateRotArrow()
    {
        if (velocity == new Vector2(0, 1))
            arrow.transform.rotation = Quaternion.Euler(Vector3.zero);
        if (velocity == new Vector2(0, -1))
            arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -180));
        if (velocity == new Vector2(-1, 0))
            arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
        if (velocity == new Vector2(1, 0))
            arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
        arrow.SetActive(move);
    }
}
