using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCell : CellGame
{
    public int nextI;
    public int nextJ;
    public int iPos;
    public int jPos;
    public int iTarget;
    public int jTarget;
    public bool unConnect;

    public void MoveToTarget(float speed)
    {
        try
        {
            position = Vector2.MoveTowards(position, MapOnline.instance.mapCells[iTarget, jTarget].position, speed * Time.deltaTime);
        }
        catch
        {
            Player.instance.move = false;
        }
    }
    public bool hasArrived()
    {
        if (DataGame.ExitRangeGame(iTarget, jTarget))
        {
            float dist = Vector2.Distance(position, MapOnline.instance.mapCells[iTarget, jTarget].position);
            return (dist < 0.1f);
        }
        else
            return false;
    }
    public void MoveNext(Vector2 velocity)
    {
        nextI = iTarget + (int)velocity.x;
        nextJ = jTarget + (int)velocity.y;
        if (hasArrived()) 
        {
            iPos = iTarget;
            jPos = jTarget;
            if (DataGame.ExitRangeGame(nextI, nextJ))
            {
                if(Player.instance.move)
                    NextCell(velocity);
            }
        }
    }
    public bool CheckNextAbc()
    {
        bool arrive = false;
        if (hasArrived() && DataGame.ExitRangeGame(nextI, nextJ))
            arrive = (MapOnline.instance.mapCells[nextI, nextJ].isAbc &&
                !MapOnline.instance.mapCells[nextI, nextJ].connectProcess) ? true : false;
        return arrive;
    }

    public bool ExitABC()
    {
        return (iPos-1 == 0 || jPos-1 == 0 || iPos+2 == DataGame.x || jPos+2 == DataGame.y);
    }

    public void Create(string typeStr, int i, int j)
    {
        iPos = iTarget = i;
        jPos = jTarget = j;
        base.Create(typeStr);
    }

    public void Disconnect()
    {
        unConnect = true;
    }

    private void Update()
    {
        if (unConnect)
        {
            if (!hasArrived())
                MoveToTarget(Player.instance.speed);
            else
            {
                MapOnline.instance.mapCells[iTarget, jTarget].SetValue(cellData.str);
                Destroy(gameObject);
            }
        }
    }
    
    public PlayerCell Connect(Vector2 velocity)
    {
        int nextI = iTarget + (int)velocity.x;
        int nextJ = jTarget + (int)velocity.y;
        string nameCell = MapOnline.instance.mapCells[nextI, nextJ].cellData.str;
        MapOnline.instance.mapCells[nextI, nextJ].Clear();
        OccupTo(nextI, nextJ);
        return CreateVagon(nameCell, nextI, nextJ);
    }

    PlayerCell CreateVagon(string str,int i, int j)
    {
        PlayerCell newVagon = Player.instance.CreatePlayerCell(str, i, j);
        newVagon.transform.SetParent(MapOnline.instance.parent);
        newVagon.transform.localScale = Vector3.one;
        newVagon.GetComponent<RectTransform>().sizeDelta = new Vector2(70, 70);
        newVagon.transform.localPosition = MapOnline.instance.mapCells[i, j].transform.localPosition;
        return newVagon;
    }

    void OccupTo(int i, int j)
    {
        MapOnline.instance.mapCells[i, j].Occup();
    }

    public void DestroyCell()
    {
        iPos = iTarget;
        jPos = jTarget;
        MapOnline.instance.mapCells[iPos, jPos].Leave();
        Destroy(gameObject);
    }

    public void LeaveTo(int i, int j)
    {
        PlayerCell prev = Player.instance.getPrev(this);
        if (prev != null)
        {
            prev.LeaveTo(prev.iTarget, prev.jTarget);
        }
        if (DataGame.ExitRangeGame(i, j) && MapOnline.instance.mapCells[i, j] != this )
            MapOnline.instance.mapCells[i, j].Leave();
    }

    public void FollowMe(int iTo, int jTo)
    {
        iPos = iTarget;
        jPos = jTarget;
        iTarget = iTo;
        jTarget = jTo;
        OccupTo(iTarget, jTarget);
        PlayerCell prev = Player.instance.getPrev(this);
        if (prev != null)
        {
            prev.FollowMe(iPos, jPos);
        }
    }

    public void NextCell(Vector2 velocity)
    {
        int nextI = iTarget + (int)velocity.x;
        int nextJ = jTarget + (int)velocity.y;
        if (DataGame.ExitRangeGame(nextI, nextJ))
        {
            if (!MapOnline.instance.mapCells[nextI, nextJ].cellData.occup)
            {
                LeaveTo(iPos, jPos);
                PlayerCell prev = Player.instance.getPrev(this);
                if (prev != null)
                {
                    prev.FollowMe(iTarget, jTarget);
                }
                iTarget += (int)velocity.x;
                jTarget += (int)velocity.y;
                OccupTo(iTarget, jTarget);
            }
        }
    }
}
