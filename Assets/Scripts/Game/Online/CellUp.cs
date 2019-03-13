using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CellUp : NetworkBehaviour
{
    [SyncVar] public string str;
    private Text textUI;
    protected Image icon;

    [SyncVar] public int nextI;
    [SyncVar] public int nextJ;
    [SyncVar] public int iPos;
    [SyncVar] public int jPos;
    [SyncVar] public int iTarget;
    [SyncVar] public int jTarget;

    private void Awake()
    {
        GetComponent<RectTransform>().sizeDelta = new Vector2(DataGame.sizeBtn, DataGame.sizeBtn);
        transform.SetParent(MapOnline.instance.parent);
        textUI = transform.Find("Text").GetComponent<Text>();
        icon = GetComponent<Image>();
        AwakeExtra();
    }
    protected virtual void AwakeExtra() { }
    public void Create(string typeStr, int i, int j)
    {
        CmdPos(i, j);
        CmdTarget(i, j);
        TextRefresh(typeStr);
    }

    [Command]
    public void CmdNext(int i, int j)
    {
        Debug.Log("next: " + i + " " + j);
        nextI = i;
        nextJ = j;
    }
    [Command]
    public void CmdPos(int i,int j)
    {
        iPos = i;
        jPos = j;
    }
    [Command]
    public void CmdTarget(int i, int j)
    {
        iTarget = i;
        jTarget = j;
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
        LoadDataNet ldn = MapOnline.instance.loadData;
        if (DataGame.ExitRangeGame(nextI, nextJ, ldn.x, ldn.y))
        {
            float dist = Vector2.Distance(position, MapOnline.instance.mapCells[iTarget, jTarget].transform.position);
            return (dist < 0.1f);
        }
        else
            return false;
    }
    public void MoveNext(Vector2 velocity)
    {
        CmdNext(iTarget + (int)velocity.x, jTarget + (int)velocity.y);
        if (hasArrived())
        {
            CmdPos(iTarget, jTarget);
            NextCell(velocity);
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
                PlayerOnline.instance.CmdLeaveTo(iTarget, jTarget);
                CellUp prev = PlayerOnline.instance.getPrev(this);
                if (prev != null)
                {
                    prev.FollowMe(iTarget, jTarget);
                }
                CmdTarget(nextI, nextJ);
                PlayerOnline.instance.CmdOccupTo(gameObject,nextI, nextJ);
            }
        }
    }
    public void FollowMe(int iTo, int jTo)
    {
        CmdPos(iTarget, jTarget);
        CmdTarget(iTo, jTo);
        PlayerOnline.instance.CmdOccupTo(gameObject,iTarget, jTarget);
        CellUp prev = PlayerOnline.instance.getPrev(this);
        if (prev != null)
        {
            prev.FollowMe(iPos, jPos);
        }
    }
}
