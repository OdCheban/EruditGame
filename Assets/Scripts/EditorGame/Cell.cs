using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public enum TypeBtn { Default, Player, Abc }
    public TypeBtn type;
    public Text txt;

    public void Create(TypeBtn type)
    {
        txt = transform.GetChild(0).GetComponent<Text>();
        this.type = type;
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
            lastAbc.UnClick();
            EditScene.instance.lastBtnCell.UnClick();
            EditScene.instance.lastBtnAbc = EditScene.instance.lastBtnCell = null;
        }
    }

    void SetValue(Cell fromTxt,Cell toTxt)
    {
        bool toPlayer = (toTxt.type == TypeBtn.Player && 
            ((fromTxt.txt.text == "player" && EditScene.instance.kPlayers < 2) || fromTxt.txt.text == ""));
        bool notToPlayer = (toTxt.type != TypeBtn.Player && (fromTxt.txt.text != "player"));
        if (notToPlayer || toPlayer)
        {
            if (toPlayer)
            {
                if (fromTxt.txt.text == "player" && toTxt.txt.text == "")
                    EditScene.instance.kPlayers++;
                else if (fromTxt.txt.text == "" && toTxt.txt.text == "player")
                    EditScene.instance.kPlayers--;
            }
            toTxt.txt.text = fromTxt.txt.text;

        }
    }

    public void UnClick()
    {
        switch (type)
        {
            case TypeBtn.Default:
            case TypeBtn.Abc:
                GetComponent<Image>().color = Color.white;
                break;
            case TypeBtn.Player:
                GetComponent<Image>().color = Color.blue;
                break;
        }
    }
}
