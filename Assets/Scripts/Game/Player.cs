using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    //состав поезда
    public List<PlayerCell> playerCells = new List<PlayerCell>();
    PlayerCell playerHead
    {
        get { return playerCells.Last(); }
    }
    public PlayerCell getPrev(PlayerCell cell)
    {
        int index = playerCells.IndexOf(cell) - 1;
        return (index >= 0)? playerCells[index]:null;
    }
    //системные характеристики
    KeyCode[] key;
    private Button btnAdd;
    private Button btnRem;

    //характеристики поезда
    float speed;
    public Vector2 velocity;
    bool move;
    Color myColor;

    //статистика поезда
    public float score;

    public void Create(string typeStr, int i, int j, Color color)
    {
        myColor = color;
        speed = DataGame.speed;
        playerCells.Add(CreateCell(typeStr,i,j));
        StartVelocity(i,j);
    }

    public void StartEngine(KeyCode[] key, Button btnAdd, Button btnRem)
    {
        ButtonActivate(btnAdd, btnRem);
        this.key = key;
        
        transform.SetAsLastSibling();
        playerHead.NextCell(velocity);
        move = true;
    }

    void Update()
    {
        if (move)
        {
            Controll();
            MovePlayer();
            btnAdd.interactable = playerHead.CheckArrive(velocity);
        }
    }

    void MovePlayer()
    {
        foreach (PlayerCell player in playerCells)
            player.MoveCell(speed);
    }

    public void Connect()
    {
        StartCoroutine(ConnectAnimation());
    }

    IEnumerator ConnectAnimation()
    {
        btnAdd.GetComponent<Image>().fillAmount = 0;
        while (btnAdd.GetComponent<Image>().fillAmount <= 0.95f)
        {
            btnAdd.GetComponent<Image>().fillAmount += 0.01f;
            yield return new WaitForEndOfFrame();
        }
        playerCells.Add(playerHead.Connect(velocity));
    }

    void ButtonActivate(Button btnAdd, Button btnRem)
    {
        btnAdd.gameObject.SetActive(true);
        btnRem.gameObject.SetActive(true);
        this.btnAdd = btnAdd;
        this.btnRem = btnRem;
        this.btnRem.onClick.RemoveAllListeners();
        this.btnAdd.onClick.RemoveAllListeners();
        this.btnRem.onClick.AddListener(() => Connect());
        this.btnAdd.onClick.AddListener(() => Connect());
    }

    void StartVelocity(int i, int j)
    {
        if (i == 0)
            velocity = new Vector2(1, 0);
        if (j == 0)
            velocity = new Vector2(0, 1);
        if (i == DataGame.x - 1)
            velocity = new Vector2(-1, 0);
        if (j == DataGame.y - 1)
            velocity = new Vector2(0, -1);
    }
     
    public PlayerCell CreateCell(string typeStr, int i, int j)
    {
        PlayerCell cell;
        if (playerCells.Count == 0)
            cell = gameObject.AddComponent<PlayerCell>();
        else
        {
            GameObject newCell = Main.instance.CreateGOcell();
            cell = newCell.AddComponent<PlayerCell>();
        }
        cell.Create(typeStr,i,j,this);
        cell.gameObject.GetComponent<Image>().color = myColor;
        return cell;
    }

    void Controll()
    {
        if (Input.GetKeyDown(key[0]))
        {
            if (velocity != new Vector2(1, 0))
                velocity = new Vector2(-1, 0);
        }
        if (Input.GetKeyDown(key[1]))
        {
            if (velocity != new Vector2(-1, 0))
                velocity = new Vector2(1, 0);
        }
        if (Input.GetKeyDown(key[2]))
        {
            if (velocity != new Vector2(0, 1))
                velocity = new Vector2(0, -1);
        }
        if (Input.GetKeyDown(key[3]))
        {
            if (velocity != new Vector2(0, -1))
                velocity = new Vector2(0, 1);
        }
        if (Input.GetKeyDown(key[4]))
        {
            Connect();
        }
    }
}
