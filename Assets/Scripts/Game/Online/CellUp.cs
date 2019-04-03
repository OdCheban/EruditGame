using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[NetworkSettings(channel = 0, sendInterval = 0)]
public class CellUp : NetworkBehaviour
{
    [SyncVar] public string str;
    private Text textUI;
    protected Image icon;
    [SyncVar] public bool unConnect;
    [SyncVar] public int nextI;
    [SyncVar] public int nextJ;
    [SyncVar] public int iPos;
    [SyncVar] public int jPos;
    [SyncVar] public int iTarget;
    [SyncVar] public int jTarget;
    [SyncVar] public bool player;
    [SyncVar] public bool destroy;

    private void Awake()
    {
        GetComponent<RectTransform>().sizeDelta = new Vector2(DataGame.sizeBtn, DataGame.sizeBtn);
        transform.SetParent(MapOnline.instance.parent);
        textUI = transform.Find("Text").GetComponent<Text>();
        icon = GetComponent<Image>();
        AwakeExtra();
    }
    protected virtual void AwakeExtra() { }

    [Command]
    public void CmdCreate(string typeStr, int i, int j)
    {
        iPos = iTarget = i;
        jPos = jTarget = j;
        TextRefresh(typeStr);
    }
    [Command]
    public void CmdDestroyCell()
    {
        iPos = iTarget;
        jPos = jTarget;
        destroy = true;
        MapOnline.instance.mapCells[iPos, jPos].RpcLeave();
        NetworkServer.Destroy(gameObject);
    }
    public Vector2 position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }

    public void TextRefresh(string m = "")
    {
        if (m != "") str = m;
        textUI.text = str;
    }
    
    public void MoveToTarget(float speed)
    {
        position = Vector2.MoveTowards(position, MapOnline.instance.mapCells[iTarget,jTarget].transform.position, speed * Time.deltaTime);
    }

    public bool hasArrived()
    {
        float dist = Vector2.Distance(position, MapOnline.instance.mapCells[iTarget, jTarget].transform.position);
        return (dist < 0.01f);
    }

    public bool checkBtn()
    {
        float dist1 = Vector2.Distance(MapOnline.instance.mapCells[0, 0].transform.position,MapOnline.instance.mapCells[0,1].transform.position);
        float dist2 = Vector2.Distance(position, MapOnline.instance.mapCells[iTarget, jTarget].transform.position);
        return (dist2 < dist1 / 2 && (dist2 > dist1/5 || dist2 == 0));
    }

    public void MoveNext(Vector2 velocity)
    {
        PlayerOnline.instance.CmdNext(gameObject, iTarget + (int)velocity.x, jTarget + (int)velocity.y);
        if (hasArrived())
        {
            NextCell(velocity);
            PlayerOnline.instance.CmdPos(gameObject, iTarget, jTarget);
        }
    }
    public void NextCell(Vector2 velocity)
    {
        int nextI = iTarget + (int)velocity.x;
        int nextJ = jTarget + (int)velocity.y;
        LoadDataNet ldn = MapOnline.instance.loadData;
        if (DataGame.ExitRangeGame(nextI, nextJ, ldn.x, ldn.y))
        {
            if (MapOnline.instance.mapCells[nextI, nextJ].upCell == null)
            {
                PlayerOnline.instance.CmdLeaveTo(gameObject, iTarget, jTarget);
                CellUp prev = PlayerOnline.instance.getPrev(this);
                if (prev != null)
                    prev.FollowMe(iTarget, jTarget);
                PlayerOnline.instance.CmdTarget(gameObject,nextI, nextJ);
                PlayerOnline.instance.CmdOccupTo(gameObject.GetComponent<NetworkIdentity>().netId, nextI, nextJ);
            }
        }
    }

    public void FollowMe(int iTo, int jTo)
    {
        PlayerOnline.instance.CmdPos(gameObject, iTarget, jTarget);
        PlayerOnline.instance.CmdTarget(gameObject, iTo, jTo);
        PlayerOnline.instance.CmdOccupTo(gameObject.GetComponent<NetworkIdentity>().netId, iTo, jTo);
        CellUp prev = PlayerOnline.instance.getPrev(this);
        if (prev != null)
        {
            prev.FollowMe(iTarget, jTarget);
        }
    }
    public CellUp Connect(Vector2 velocity)
    {
        int nextI = iTarget + (int)velocity.x;
        int nextJ = jTarget + (int)velocity.y;
        PlayerOnline.instance.CmdNext(gameObject, nextI,nextJ);
        CellUp cellUp = MapOnline.instance.mapCells[nextI, nextJ].upCell;
        PlayerOnline.instance.CmdPos(cellUp.gameObject, nextI, nextJ);
        PlayerOnline.instance.CmdTarget(cellUp.gameObject, nextI, nextJ);
        return cellUp;
    }

    public bool CheckNextAbc()
    {
        LoadDataNet ldn = MapOnline.instance.loadData;
        if (hasArrived() && DataGame.ExitRangeGame(nextI, nextJ, ldn.x, ldn.y))
        {
            if (MapOnline.instance.mapCells[nextI, nextJ].upCell != null && !MapOnline.instance.mapCells[nextI, nextJ].upCell.player)
            {
                return (MapOnline.instance.mapCells[nextI, nextJ].upCell.isAbc() &&
                    !MapOnline.instance.mapCells[nextI, nextJ].connectProcess);
            }
        }
        return false;
    }
    public bool isAbc()
    {
        return (str.Length == 1);
    }

    [ClientRpc]
    public void RpcDisconnectCell()
    {
        unConnect = true;
        GetComponent<Image>().color = Color.white;
    }
    public bool ExitABC()
    {
        return (iPos == 0 || jPos == 0 || iPos == MapOnline.instance.loadData.x-1 || jPos == MapOnline.instance.loadData.y-1);
    }
    private void Update()
    {
        if (unConnect)
        {
            if (!hasArrived())
                MoveToTarget(MapOnline.instance.loadData.speed);
            else
            {
                PlayerOnline.instance.CmdUnConnectEnd(gameObject, iTarget, jTarget);
            }
        }
    }
}
