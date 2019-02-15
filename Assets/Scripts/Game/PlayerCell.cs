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

    public void MoveCell(float speed)
    {
        MoveTo(iTarget, jTarget, speed);
    }

    void MoveTo(int i, int j, float speed)
    {
        position = Vector2.MoveTowards(position, Main.instance.cells[i, j].position, speed * Time.deltaTime);
    }

    public bool CheckArrive(Vector2 velocity)
    {
        bool arrive = false;
        float dist = Vector2.Distance(position, Main.instance.cells[iTarget, jTarget].position);
        int nextI = iTarget + (int)velocity.x;
        int nextJ = jTarget + (int)velocity.y;
        if (dist < 0.1f)
        {
            if (DataGame.ExitRangeGame(nextI, nextJ))
            {
                arrive = (Main.instance.cells[nextI, nextJ].isAbc) ? true : false;
                if (!Main.instance.cells[nextI, nextJ].occup)
                {
                    LeaveTo(iTarget, jTarget);
                    NextCell(velocity);
                }
            }
        }
        return arrive;
    }

    public void Create(string typeStr, int i, int j,Player player)
    {
        this.player = player;
        iPos = iTarget = i;
        jPos = jTarget = j;
        base.Create(typeStr);
    }

    public PlayerCell Connect(Vector2 velocity)
    {
        int nextI = iTarget + (int)velocity.x;
        int nextJ = jTarget + (int)velocity.y;
        string nameCell = Main.instance.cells[nextI, nextJ].str;
        Main.instance.cells[nextI, nextJ].Clear();
        return CreateVagon(nameCell, nextI, nextJ);
    }

    PlayerCell CreateVagon(string str,int i, int j)
    {
        PlayerCell newVagon = player.CreateCell(str, i, j);
        newVagon.transform.localPosition = Main.instance.cells[i, j].transform.localPosition;
        return newVagon;
    }

    void OccupTo(int i, int j)
    {
        Main.instance.cells[i, j].Occup();
    }

    public void LeaveTo(int i, int j)
    {
        iPos = iTarget;
        jPos = jTarget;
        PlayerCell prev = player.getPrev(this);
        if (prev != null)
        {
            prev.LeaveTo(prev.iTarget, prev.jTarget);
        }
        if (Main.instance.cells[i, j] != this)
            Main.instance.cells[i, j].Leave();
    }

    public void FollowMe(int iTo, int jTo)
    {
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
