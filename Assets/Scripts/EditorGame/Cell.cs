using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public enum TypeBtn { Default, Player, Abc }
    public TypeBtn type;
    public Text txt;
    public string text
    {
        set { txt.text = value; }
        get { return txt.text; }
    }
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
            EditScene.instance.lastBtnCell.UnClick();
            EditScene.instance.lastBtnCell = null;
        }
    }

    void SetValue(Cell fromTxt,Cell toTxt)
    {
        bool toPlayer = (toTxt.type == TypeBtn.Player && 
            ((fromTxt.text == "player" && EditScene.instance.kPlayers < 2) || fromTxt.text == ""));
        bool notToPlayer = (toTxt.type != TypeBtn.Player && (fromTxt.text != "player"));
        if (notToPlayer || toPlayer)
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
