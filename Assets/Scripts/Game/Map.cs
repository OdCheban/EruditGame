using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Map : MonoBehaviour {

    public delegate GameObject CreateCellFunc(float sizeBtn);

    public static CellGame[,] CreateMap(int x, int y, float sizeBtn, List<List<string>> map,CreateCellFunc CreateFunc)
    {
        CellGame[,] mapSc = new CellGame[x, y];
        int kPlayers = 0;
        for (int i = 0; i < x; i++)
            for (int j = 0; j < y; j++)
                    mapSc[i,j] = CreateCell(ref kPlayers, i, j, map[i][j], x, y, sizeBtn, CreateFunc);

        return mapSc;
    }

    public static CellGame CreateCell(ref int N, int i, int j, string typeStr, int width, int height, float sizeBtn, CreateCellFunc CreateFunc)
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
                Player player = btn.AddComponent<Player>();
                player.Create(N, typeStr, i, j, DataGame.colorPlayers[N]);
                N++;
                //players.Add(player);
                CellGame cellGame = btn.AddComponent<CellGame>();
                cellGame.Create(typeStr);
                return cellGame;
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

    public static void StartPosition(CellGame[,] mas,Transform parent, int width,int height,float sizeBtn)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (mas[i, j] != null)
                {
                    mas[i, j].transform.SetParent(parent);
                    mas[i, j].GetComponent<RectTransform>().sizeDelta = new Vector2(sizeBtn, sizeBtn);
                    Vector2 offset = new Vector2(-width * sizeBtn / 2, height * sizeBtn / 2);
                    mas[i, j].transform.position = offset + new Vector2((i + 0.5f) * (sizeBtn+0.2f), -(j + 0.5f) * (sizeBtn + 0.2f));
                }
                else
                    Debug.Log(i + " " + j);
            }
        }
    }

    public static GameObject CreateGOcell(float sizeBtn)
    {
        GameObject cellGO = (GameObject)Instantiate(Resources.Load("Cell"));
        cellGO.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeBtn, sizeBtn);
        cellGO.transform.localScale = Vector3.one;
        return cellGO;
    }
}
