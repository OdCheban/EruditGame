using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellGame : MonoBehaviour {
    public bool occup;
    public Vector2 position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }
    public string str
    {
        get { return txt.text; }
        set { txt.text = value; }
    }
    public bool isAbc
    {
        get { return (str != "wall" && str != ""); }
    }
    private Text txt;
    public bool connectProcess;

    private void Awake()
    {
        txt = transform.GetChild(0).GetComponent<Text>();
    }
    public virtual void Create(string typeStr)
    {
        if (typeStr != "cell")
        {
            str = typeStr;
            occup = true;
        }
    }
    public void Occup()
    {
        occup = true;
        GetComponent<Image>().color = Color.gray;
    }
    public void Leave()
    {
        occup = false;
        GetComponent<Image>().color = Color.white;
    }

    public void SetValue(string m)
    {
        GetComponent<Image>().color = Color.white;
        occup = true;
        str = m;
    }
    public void Clear()
    {
        occup = false;
        str = "";
    }

}
