using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Map : MonoBehaviour {

    public delegate GameObject CreateCellFunc(int sizeBtn);

    public static CellGame[,] CreateMap(int x, int y, int sizeBtn, List<List<string>> map,CreateCellFunc CreateFunc)
    {
        CellGame[,] mapSc = new CellGame[x, y];
        
        for (int i = 0; i < x; i++)
            for (int j = 0; j < y; j++)
                    mapSc[i,j] = CreateCell(i, j, map[i][j], x, y, sizeBtn, CreateFunc);

        return mapSc;
    }
    public static float ScaleParent(Transform parent, int x, int y)
    {
        float max = Mathf.Max(x, y);
        float scale = 15.0f / max;
        parent.localScale = new Vector2(scale, scale);
        return scale;
    }
    public static CellGame CreateCell(int i, int j, string typeStr, int width, int height, int sizeBtn, CreateCellFunc CreateFunc)
    {
        if (typeStr != "null")
        {
            GameObject btn = CreateFunc(sizeBtn);
            if (typeStr != "player")
            {
                CellGame cellGame = btn.AddComponent<CellGame>();
                cellGame.Create(typeStr);
                return cellGame;
            }
            else
            {
                //Player player = btn.AddComponent<Player>();
                //player.Create(typeStr, i, j,DataGame.colorPlayers[players.Count]);
                //players.Add(player);
                Debug.Log("!!!");
                return null;
            }
        }
        else
        {
            GameObject btn = CreateFunc(sizeBtn);
            CellGame cellGame = btn.AddComponent<CellGame>();
            cellGame.Create(typeStr);
            return cellGame;
        }
    }

    public static void RefreshPosition(GameObject[,] mas,Transform parent, int width,int height,int sizeBtn)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (mas[i, j] != null)
                {
                    mas[i, j].transform.SetParent(parent);
                    Vector2 offset = new Vector2(-width * sizeBtn / 2, height * sizeBtn / 2);
                    mas[i, j].transform.localScale = Vector3.one;
                    mas[i, j].GetComponent<RectTransform>().sizeDelta = new Vector2(sizeBtn, sizeBtn);
                    mas[i, j].transform.localPosition = offset + new Vector2((i + 0.5f) * sizeBtn, -(j + 0.5f) * sizeBtn);
                }
                else
                    Debug.Log(i + " " + j);
            }
        }
    }

    public static GameObject CreateGOcell(int sizeBtn)
    {
        GameObject cellGO = (GameObject)Instantiate(Resources.Load("Cell"));
        cellGO.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeBtn, sizeBtn);
        cellGO.transform.localScale = Vector3.one;
        return cellGO;
    }
}
