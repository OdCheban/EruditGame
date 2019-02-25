using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DataGame : MonoBehaviour {

    private static int _x;
    private static int _y;
    public static Color[] colorPlayers = new Color[2] { Color.blue, Color.red };
    public static KeyCode[,] key = new KeyCode[2,7] { { KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S,KeyCode.Z,KeyCode.X,KeyCode.Q },{ KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.DownArrow,KeyCode.RightShift,KeyCode.M,KeyCode.Space } };

    public static List<List<string>> map;
    public static List<string> abcResult;
    public static int y
    {
        get { return _y + 2; }
        set { _y = value; }
    }
    public static int x
    {
        get { return _x + 2; }
        set { _x = value; }
    }
    public static int modeMove;//0(hard)/1(easy)
    public static float timeExit;
    public static float timeGame;
    public static float speed;
    public static float speedVagon;
    public static float speedConnect;
    public static float speedDisconnect;
    public static float xBonus;
    public static int sizeBtn = 60;

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
    public static List<string> allWords = new List<string>();

    public static bool ExitRange(int i, int j, int iMax = 0, int jMax = 0)
    {
        if (iMax == 0) { iMax = x; jMax = y; }
        return ((i != 0 || j != 0) &&
                   (i != iMax - 1 || j != 0) &&
                   (i != 0 || j != jMax - 1) &&
                   (i != iMax - 1 || j != jMax - 1));
    }
    public static bool ExitRangeGame(int i, int j)
    {
        return (i != 0 && j != 0) && (i < x - 1 && j < y - 1);
    }

    static void ReadDictonaryFromFile()
    {
        StreamReader objReader = new StreamReader(Application.dataPath + "/dictonary.txt");
        string sLine = "";
        while (sLine != null)
        {
            sLine = objReader.ReadLine();
            if (sLine != null)
                allWords.Add(sLine);
        }
    }

    public static void LoadData()
    {
        ReadDictonaryFromFile();
        x = PlayerPrefs.GetInt("x", 5);
        y = PlayerPrefs.GetInt("y", 5);
        modeMove = PlayerPrefs.GetInt("move", 0);
        timeExit = PlayerPrefs.GetFloat("timeExit", 2.0f);
        timeGame = PlayerPrefs.GetFloat("timeGame", 60);
        speed = PlayerPrefs.GetFloat("speed", 1.5f);
        speedVagon = PlayerPrefs.GetFloat("speedVagon", 0.2f);
        speedConnect = PlayerPrefs.GetFloat("speedConnect", .03f);
        speedDisconnect = PlayerPrefs.GetFloat("speedDisconnect", .03f);
        xBonus = PlayerPrefs.GetFloat("xBonus", 1.5f);
        abcResult = PlayerPrefs.GetString("words", "слова мяч огонь вода яд").Split(' ','\n').ToList();
        string m = PlayerPrefs.GetString("map", "");
        if (m == "")
        {
            map = new List<List<string>>()
            {
                new List<string>() { "null", "null", "null", "null", "null" },
                new List<string>() { "null", "cell", "cell", "cell", "cell", "cell", "null" },
                new List<string>() { "null", "cell", "cell", "cell", "cell", "cell", "player" },
                new List<string>() { "null", "cell", "cell", "cell", "cell", "cell", "null" },
                new List<string>() { "null", "cell", "cell", "cell", "cell", "cell", "null" },
                new List<string>() { "null", "cell", "cell", "cell", "cell", "cell", "null" },
                new List<string>() { "null", "null", "null", "null", "null" }
            };
        }
        else
        {
            string[] mm = m.Split(' ');
            List<List<string>> m_map = new List<List<string>>();
            int k = 0;
            for (int i = 0; i < x; i++)
            {
                List<string> str = new List<string>();
                for (int j = 0; j < y; j++)
                {
                    if (ExitRange(i, j, x, y))
                    {
                        str.Add(mm[k]);
                        k++;
                    }
                }
                m_map.Add(str);
            }
            map = m_map;


        }
    }

}
