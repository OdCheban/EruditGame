using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCell : CellGame
{
    private Player player;
    //stats
    public int iPos;
    public int jPos;
    public int iTarget;
    public int jTarget;
    public bool unConnect;

    public void MoveCell(float speed)
    {
        MoveTo(iTarget, jTarget, speed);
    }

    void MoveTo(int i, int j, float speed)
    {
        try
        {
            position = Vector2.MoveTowards(position, MapOffline.instance.cells[i, j].position, speed * Time.deltaTime);
        }catch
        {
            player.move = false;
        }
    }

    public bool hasArrived()
    {
        if (DataGame.ExitRangeGame(iTarget, jTarget))
        {
            float dist = Vector2.Distance(position, MapOffline.instance.cells[iTarget, jTarget].position);
            return (dist < 0.1f);
        }
        else
            return false;
    }
    float last = 0;
    public bool CheckArrive(Vector2 velocity,Player.MoveMode moveMode)
    {
        bool arrive = false;
        int nextI = iTarget + (int)velocity.x;
        int nextJ = jTarget + (int)velocity.y;
        if (hasArrived()) 
        {
            iPos = iTarget;
            jPos = jTarget;
            if (DataGame.ExitRangeGame(nextI, nextJ))
            {
                last = Time.fixedTime;
                arrive = (MapOffline.instance.cells[nextI, nextJ].isAbc && !MapOffline.instance.cells[nextI, nextJ].connectProcess) ? true : false;
                if(player.move)
                NextCell(velocity);
            }
        }
        return arrive;
    }

    public bool ExitABC()
    {
        return (iPos-1 == 0 || jPos-1 == 0 || iPos+2 == DataGame.x || jPos+2 == DataGame.y);
    }

    public void Create(string typeStr, int i, int j,Player player)
    {
        this.player = player;
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
                MoveCell(player.speed);
            else
            {
                MapOffline.instance.cells[iTarget, jTarget].SetValue(cellData.str);
                Destroy(gameObject);
            }
        }
    }


    public PlayerCell Connect(Vector2 velocity)
    {
        int nextI = iTarget + (int)velocity.x;
        int nextJ = jTarget + (int)velocity.y;
        string nameCell = MapOffline.instance.cells[nextI, nextJ].cellData.str;
        MapOffline.instance.cells[nextI, nextJ].Clear();
        OccupTo(nextI, nextJ);
        return CreateVagon(nameCell, nextI, nextJ);
    }

    PlayerCell CreateVagon(string str,int i, int j)
    {
        PlayerCell newVagon = player.CreateCell(str, i, j);
        newVagon.transform.localPosition = MapOffline.instance.cells[i, j].transform.localPosition;
        return newVagon;
    }

    void OccupTo(int i, int j)
    {
        MapOffline.instance.cells[i, j].Occup();
    }

    public void DestroyCell()
    {
        iPos = iTarget;
        jPos = jTarget;
        MapOffline.instance.cells[iPos, jPos].Leave();
        Destroy(gameObject);
    }

    public void LeaveTo(int i, int j)
    {
        PlayerCell prev = player.getPrev(this);
        if (prev != null)
        {
            prev.LeaveTo(prev.iTarget, prev.jTarget);
        }
        if (DataGame.ExitRangeGame(i, j) && MapOffline.instance.cells[i, j] != this )
            MapOffline.instance.cells[i, j].Leave();
    }

    public void FollowMe(int iTo, int jTo)
    {
        iPos = iTarget;
        jPos = jTarget;
        iTarget = iTo;
        jTarget = jTo;
        OccupTo(iTarget, jTarget);
        PlayerCell prev = player.getPrev(this);
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
            if (!MapOffline.instance.cells[nextI, nextJ].cellData.occup)
            {
                LeaveTo(iPos, jPos);
                PlayerCell prev = player.getPrev(this);
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
