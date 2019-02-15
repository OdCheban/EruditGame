using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataGame : MonoBehaviour {

    private static int _x = 5;
    public static int x
    {
        get { return _x + 2; }
        set { _x = value; }
    }
    private static int _y = 5;
    public static int y
    {
        get { return _y + 2; }
        set { _y = value; }
    }
    public static float speed = 80.0f;
    public static float timeGame = 60.0f;
    public static Color[] colorPlayers = new Color[2] { Color.blue, Color.red };
    public static KeyCode[,] key = new KeyCode[2,6] { { KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S,KeyCode.Z,KeyCode.X },{ KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.DownArrow,KeyCode.RightShift,KeyCode.M } };
    public static bool ExitRange(int i, int j,int iMax = 0, int jMax = 0)
    {
        if (iMax == 0) { iMax = x; jMax = y; }
        return ((i != 0 || j != 0) &&
                   (i != iMax-1 || j != 0) &&
                   (i != 0 || j != jMax-1 ) &&
                   (i != iMax-1 || j != jMax-1));
    }
    public static bool ExitRangeGame(int i, int j)
    {
        return (i != 0 && j != 0) && (i < x-1 && j < y-1);
    }

}
