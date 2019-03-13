using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public int i;
    public int j;
    public Text txt;
    public string text
    {
        set { txt.text = value; }
        get { return txt.text; }
    }
    public void Create(int i = -1,int j = -1)
    {
        this.i = i;
        this.j = j;
        txt = transform.GetChild(0).GetComponent<Text>();
        UnClick();
    }

    public void Click()
    {
        GetComponent<Image>().color = Color.red;
        Cell lastAbc = EditScene.instance.lastBtnAbc;
        Cell lastCell = EditScene.instance.lastBtnCell;
        if (lastCell != null && lastAbc != null)
        {
            SetValue(lastAbc, lastCell);
            EditScene.instance.lastBtnCell.UnClick();
            EditScene.instance.lastBtnCell = null;
        }
    }

    void SetValue(Cell fromTxt,Cell toTxt)
    {
        bool exitRange = (j == 0|| i == 0 || EditScene.instance.x == i+1 || EditScene.instance.y == j+1);
        bool toPlayer = ((fromTxt.text == "player" && EditScene.instance.kPlayers < 2) || fromTxt.text == "");
        bool notToPlayer = fromTxt.text != "player";
        if (notToPlayer || (toPlayer&&exitRange))
        {
            if (toPlayer)
            {
                if (fromTxt.text == "player" && toTxt.text == "")
                    EditScene.instance.kPlayers++;
                else if (fromTxt.text == "" && toTxt.text== "player")
                    EditScene.instance.kPlayers--;
            }
            toTxt.text = fromTxt.text;
        }

        //выводим все буквы
        EditScene.instance.CheckAvialWords();
    }

    public void UnClick()
    {
        GetComponent<Image>().color = Color.white;
    }
}
