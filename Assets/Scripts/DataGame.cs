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
    public static int move;//0 без остановок 1 с остановками
    public static Color[] colorPlayers = new Color[2] { Color.blue, Color.red };
    public static KeyCode[,] key = new KeyCode[2,7] { { KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S,KeyCode.Z,KeyCode.X,KeyCode.Q },{ KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.DownArrow,KeyCode.RightShift,KeyCode.M,KeyCode.Space } };
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

    public static float timeExit = 5.0f;
    public static float timeGame = 60.0f;
    public static float speed = 140.0f;
    public static float speedVagon = 10.0f;
    public static float speedConnect = 3.0f;
    public static float speedDisconnect = 5.0f;
    public static float xBonus = 1;
    public static Dictionary<string,int> ABC = new Dictionary<string, int>() {
        { "a", 1 },
        { "б", 3 },
        { "в", 2 },
        { "г", 3 },
        { "д", 2 },
        { "е", 1 },
        { "ж", 5 },
        { "з", 5 },
        { "и", 1 },
        { "й", 2 },
        { "к", 2 },
        { "л", 2 },
        { "м", 2 },
        { "н", 1 },
        { "о", 1 },
        { "п", 2 },
        { "р", 2 },
        { "с", 2 },
        { "т", 2 },
        { "у", 3 },
        { "ф", 10 },
        { "х", 5 },
        { "ц", 10 },
        { "ч", 5 },
        { "ш", 10 },
        { "щ", 10 },
        { "ъ", 10 },
        { "ы", 5 },
        { "ь", 5 },
        { "э", 10 },
        { "ю", 10 },
        { "я", 3 },
    };
}
