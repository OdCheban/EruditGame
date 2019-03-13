using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct CellGam
{
    public bool occup;
    public string str;
    public bool connectProcess;
}

public class CellGame : MonoBehaviour {
    public CellGam cellData = new CellGam();
    public CellGame cellUp;
    public Vector2 position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }
    public bool isAbc
    {
        get { return (cellData.str != "wall" && cellData.str != ""); }
    }
    public bool connectProcess;
    private Text txt;

    private void Awake()
    {
        txt = transform.GetChild(0).GetComponent<Text>();
    }
    public void GetData(CellGam cellData)
    {
        this.cellData = cellData;
        txt.text = cellData.str;
    }
    public virtual void Create(string typeStr)
    {
        if (typeStr != "cell")
        {
            cellData.str = typeStr;
            txt.text = cellData.str;
            cellData.occup = true;
        }
    }
    public void Occup()
    {
        cellData.occup = true;
        GetComponent<Image>().color = Color.gray;
    }
    public void Leave()
    {
        cellData.occup = false;
        GetComponent<Image>().color = Color.white;
    }

    public void SetValue(string m)
    {
        GetComponent<Image>().color = Color.white;
        cellData.occup = true;
        cellData.str = txt.text = m;
    }
    public void Clear()
    {
        cellData.occup = false;
        cellData.str = txt.text = "";
    }

}
