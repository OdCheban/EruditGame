using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugUI : MonoBehaviour {
    public static DebugUI instance;
    public Text txt;
    private void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void SetText(string m)
    {
        txt.text += m;
    }
}
