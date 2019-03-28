using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


public class DataGame : MonoBehaviour
{
    public static bool loadData;
    private static int _x;
    private static int _y;
    public static Color[] colorPlayers = new Color[4] { Color.blue, Color.red, Color.green, Color.yellow };
    public static KeyCode[] key = new KeyCode[7] { KeyCode.D, KeyCode.A, KeyCode.S, KeyCode.W, KeyCode.Z, KeyCode.X, KeyCode.Q };

    public static string str_map
    {
        get
        {
            string str = "";
            foreach (List<string> lst in map)
            {
                foreach (string s in lst)
                    str += s + " ";
            }
            return str;
        }
    }
    public static List<List<string>> map;
    public static uint kPLayers
    {
        get
        {
            uint k = 0;
            foreach (List<string> lst in map)
                foreach (string s in lst)
                    if (s == "player")
                        k++;
            return k;
        }
    }
    public static List<string> abcResult;
    public static int maxJ
    {
        get { return _y; }
        set { _y = value; }
    }
    public static int maxI
    {
        get { return _x; }
        set { _x = value; }
    }
    public static int modeMove;//0(hard)/1(easy)
    public static int timeExit;
    public static float timeGame;
    public static float speed;
    public static float speedVagon;
    public static float speedConnect;
    public static float speedDisconnect;
    public static float xBonus;
    public static float sizeBtn = 1.0f;

    public static Dictionary<char, int> ABCscore = new Dictionary<char, int>() {
        { 'а', 1 },
        { 'б', 3 },
        { 'в', 2 },
        { 'г', 3 },
        { 'д', 2 },
        { 'е', 1 },
        { 'ж', 5 },
        { 'з', 5 },
        { 'и', 1 },
        { 'й', 2 },
        { 'к', 2 },
        { 'л', 2 },
        { 'м', 2 },
        { 'н', 1 },
        { 'о', 1 },
        { 'п', 2 },
        { 'р', 2 },
        { 'с', 2 },
        { 'т', 2 },
        { 'у', 3 },
        { 'ф', 10 },
        { 'х', 5 },
        { 'ц', 10 },
        { 'ч', 5 },
        { 'ш', 10 },
        { 'щ', 10 },
        { 'ъ', 10 },
        { 'ы', 5 },
        { 'ь', 5 },
        { 'э', 10 },
        { 'ю', 10 },
        { 'я', 3 },
        { 'ё', 15 }
    };
    public static List<string> abc = new List<string>();
    public static bool ExitRange(int i, int j, int iMax = 0, int jMax = 0)
    {
        if (iMax == 0) { iMax = maxI; jMax = maxJ; }
        return ((i != 0 || j != 0) &&
                   (i != iMax - 1 || j != 0) &&
                   (i != 0 || j != jMax - 1) &&
                   (i != iMax - 1 || j != jMax - 1));
    }
    public static bool ExitRangeGame(int i, int j, int maxI = -1, int maxJ = -1)
    {
        int iMax = DataGame.maxI;
        int jMax = DataGame.maxJ;
        if (maxI != -1)
        {
            iMax = maxI;
            jMax = maxJ;
        }
        return (i >= 0 && j >= 0) && (i < iMax && j < jMax);
    }

    public static void ReadDictonaryFromFile()
    {
        TextAsset txtAsset = (TextAsset)Resources.Load("dictonary");
        string[] tt = txtAsset.text.Split('\n');
        for (int i = 0; i < tt.Length; i++)
            abc.Add(tt[i].Trim());
    }

    public static void LoadData()
    {
        maxI = PlayerPrefs.GetInt("x", 5);
        maxJ = PlayerPrefs.GetInt("y", 5);
        modeMove = PlayerPrefs.GetInt("move", 0);
        timeExit = PlayerPrefs.GetInt("timeExit", 2);
        timeGame = PlayerPrefs.GetFloat("timeGame", 60);
        speed = PlayerPrefs.GetFloat("speed", 1.5f);
        speedVagon = PlayerPrefs.GetFloat("speedVagon", 0.2f);
        speedConnect = PlayerPrefs.GetFloat("speedConnect", .03f);
        speedDisconnect = PlayerPrefs.GetFloat("speedDisconnect", .03f);
        xBonus = PlayerPrefs.GetFloat("xBonus", 1.5f);
        abcResult = PlayerPrefs.GetString("words", "").Split(' ', '\n').ToList();
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
            map = StrToListMap(m, maxI, maxJ);
        }
        loadData = true;
    }

    public static List<List<string>> StrToListMap(string str, int x, int y)
    {
        string[] mm = str.Split(' ');
        List<List<string>> m_map = new List<List<string>>();
        int k = 0;
        for (int i = 0; i < x; i++)
        {
            List<string> strLine = new List<string>();
            for (int j = 0; j < y; j++)
            {
                //if (ExitRange(i, j, x, y))
                //{
                strLine.Add(mm[k]);
                k++;
                //}
            }
            m_map.Add(strLine);
        }
        return m_map;
    }

}
