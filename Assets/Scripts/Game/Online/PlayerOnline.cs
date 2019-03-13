using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerOnline : CellUp
{
    public static PlayerOnline instance;
    public GameObject arrow;

    [SyncVar] public bool move;
    [SyncVar] public Vector2 velocity;
    [SyncVar] public int k;
    public List<CellUp> playerCells;
    CellUp playerHead
    {
        get { return playerCells.Last(); }
    }
    public CellUp getPrev(CellUp cell)
    {
        int index = playerCells.IndexOf(cell) - 1;
        return (index >= 0) ? playerCells[index] : null;
    }

    protected override void AwakeExtra()
    {
        arrow = transform.Find("Arrow").gameObject;
        arrow.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(DataGame.sizeBtn, DataGame.sizeBtn);
        arrow.transform.GetChild(0).localPosition = Vector3.zero;
        arrow.SetActive(false);
        playerCells.Add(this);
    }
    private void Start()
    {
        if (isLocalPlayer) instance = this;
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
    public void CmdMove(bool move)
    {
        this.move = move;
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            Controll();
            if (move)
            {
                MovePlayer();
                playerHead.MoveNext(velocity);
            }
        }
        UpdateRotArrow();
    }

    void MovePlayer()
    {
        foreach (CellUp player in playerCells)
            player.MoveToTarget(MapOnline.instance.loadData.speed);
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
        if (move)
            arrow.SetActive(true);
    }

    [Command]
    public void CmdOccupTo(GameObject obj, int i, int j)
    {
        MapOnline.instance.mapCells[i, j].RpcOccup(obj);
    }
    [Command]
    public void CmdLeaveTo(int i, int j)
    {
        CellUp prev = getPrev(this);
        if (prev != null)
        {
            CmdLeaveTo(prev.iTarget, prev.jTarget);
        }
        LoadDataNet ldn = MapOnline.instance.loadData;
        if (DataGame.ExitRangeGame(nextI, nextJ, ldn.x, ldn.y))
            MapOnline.instance.mapCells[i, j].RpcLeave();
    }
}
