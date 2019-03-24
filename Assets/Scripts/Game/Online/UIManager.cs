using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : NetworkBehaviour {
    public static UIManager instance;
    public NetworkManager manager;

    public string nameConnRoom;

    [Header("UI:")]
    public InputField textFieldRoom;
    public GameObject player_ui;
    public GameObject worldRoom_ui;
    public GameObject finish_ui;
    public Text txtResult;
    public GameObject room_ui;
    public Text roomInfo;
    public Text roomChat;

    public Button add_btn;
    public Button rem_btn;
    [HideInInspector] public Image rem_img;
    [HideInInspector] public Image add_img;
    public Text points_text;

    private void Awake()
    {
        instance = this;
        rem_img = rem_btn.GetComponent<Image>();
        add_img = add_btn.GetComponent<Image>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MenuScene");
        }
    }

    public void Exit()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public Transform contentRoom;
    public List<GameObject> listRoom;

    public Text debugInfo;
    private void Start()
    {
        manager = NetworkManager.singleton;
        if (manager.matchMaker == null)
        {
            manager.StartMatchMaker();
        }
        RefreshRoom();
    }


    public void DeleteListRoom()
    {
        for (int i = 0; i < listRoom.Count; i++)
            Destroy(listRoom[i]);
        listRoom.Clear();
    }
    
    public void CreateListRoom(string nameRoom,int size, int n, UnityEngine.Networking.Types.NetworkID id)
    {
        GameObject roomObj = (GameObject) Instantiate(Resources.Load("Element"),contentRoom);
        Button btnRoom = roomObj.GetComponent<Button>();
        btnRoom.onClick.AddListener(() => {
            nameConnRoom = textFieldRoom.text;
            NetworkServer.Listen(7777);
            NetworkManager.singleton.matchMaker.JoinMatch(id, "", "", "", 0, 0, OnjoinedMatch);
            EnterRoom();
        } );
        Text txtRoom = roomObj.transform.GetChild(0).GetComponent<Text>();
        txtRoom.text = nameRoom + ":" + n + "/" + size;
        listRoom.Add(roomObj);
    }

    [ClientRpc]
    public void RpcRoomInfo(int k,int maxK,string nameRoom)
    {
        roomInfo.text = k + "/" + maxK + " " + nameRoom;
    }
    [ClientRpc]
    public void RpcRoomChat(int sec)
    {
        roomChat.text += "сервер:" + sec+ "..." +"\n";
    }
    [ClientRpc]
    public void RpcStartRoom()
    {
        Camera.main.transform.position = new Vector3(0, 0, -10);
        worldRoom_ui.SetActive(false);
        room_ui.SetActive(false);
        player_ui.SetActive(true);
        rem_btn.onClick.AddListener(() => PlayerOnline.instance.Disconnect());
        add_btn.onClick.AddListener(() => PlayerOnline.instance.Connect());
    }
    public void CreateRoom()
    {
        Debug.Log("createRoom " + textFieldRoom.text);
        if (manager.matchMaker == null)
        {
            manager.StartMatchMaker();
        }
        CreateInternetMatch(textFieldRoom.text,DataGame.kPLayers);
        nameConnRoom = textFieldRoom.text;
        EnterRoom();
    }

    public void RefreshRoom()
    {
        if (manager.matchMaker == null)
        {
            manager.StartMatchMaker();
        }
        DeleteListRoom();
        GetListRoom();
    }

    public void LeaveRoom()
    {
        Camera.main.transform.position = new Vector3(0, 0, -10);
        worldRoom_ui.SetActive(true);
        room_ui.SetActive(false);
        player_ui.SetActive(false);
        NetworkManager.singleton.StopHost();
    }
    void EnterRoom()
    {
        Camera.main.transform.position = new Vector3(3, 0, -15);
        worldRoom_ui.SetActive(false);
        room_ui.SetActive(true);
        player_ui.SetActive(false);
    }

    [Command]
    public void CmdEndGame()
    {
        RpcEndGame();
        CustomNetManager.instance.playerss.Sort((emp1, emp2)=>emp1.score.CompareTo(emp2.score));
        for (int i = 0; i < CustomNetManager.instance.playerss.Count; i++)
            RpcTableRecords(i, StrColor(DataGame.colorPlayers[CustomNetManager.instance.playerss[i].k]), CustomNetManager.instance.playerss[i].score);
    }
    [ClientRpc]
    public void RpcEndGame()
    {
        player_ui.SetActive(false);
        finish_ui.SetActive(true);
    }

    [ClientRpc]
    public void RpcTableRecords(int i, string m, float score)
    {
        txtResult.text += i+")"+ m + " получил " + score + " очков!" + "\n";
    }
    private string StrColor(Color color)
    {
        if (color == Color.red)
            return "<color=#ff0000ff>Красный</color>";
        if (color == Color.blue)
            return "<color=#0000ffff>Синий</color>";
        return "???";
    }

    public void CreateInternetMatch(string matchName,uint sizeN)
    {
        Debug.Log("create " + matchName);
        if (manager.matchMaker == null)
        {
            manager.StartMatchMaker();
        }
        NetworkManager.singleton.matchMaker.CreateMatch(matchName, sizeN, true, "", "", "", 0, 0, OnMatchCreate);
    }
    //Create Match success calback
    public void OnMatchCreate(bool success, string info, MatchInfo matchInfoData)
    {
        if (success && matchInfoData != null)
        {
            NetworkServer.Listen(matchInfoData, 7777);
            NetworkManager.singleton.networkPort = 7777;
            NetworkManager.singleton.StartHost(matchInfoData);
        }
        else
        {
            LeaveRoom();
            Debug.LogError("Create match failed : " + success + ", " + info);
        }
    }
    public void FindInternetMatch(string matchName)
    {
        Debug.Log("Find " + matchName);
        NetworkManager.singleton.matchMaker.ListMatches(0, 20, matchName, false, 0, 0, OnMatchListFound);
    }
    public void GetListRoom()
    {
        NetworkManager.singleton.matchMaker.ListMatches(0, 10, "", true, 0, 0, OnMatchListFound);
    }
    void OnMatchListFound(bool success, string info, List<MatchInfoSnapshot> matchInfoSnapshotLst)
    {
        foreach (MatchInfoSnapshot matchName in matchInfoSnapshotLst)
            CreateListRoom(matchName.name,matchName.maxSize,matchName.currentSize,matchName.networkId);
        if (success)
        {
            if (matchInfoSnapshotLst.Count != 0)
            {
                //Debug.Log("A list of matches was returned");
                //join the last server (just in case there are two...)
                //matchMaker.JoinMatch(matchInfoSnapshotLst[0].networkId, "", "", "", 0, 0, OnjoinedMatch);
            }
            else
            {
                // Debug.Log("No matches in requested room!");
            }
        }
        else
        {
            Debug.LogError("Couldn't connect to match maker");
        }
    }
    void OnjoinedMatch(bool success, string info, MatchInfo matchInfoData)
    {
        Debug.Log("find " + info);
        if (success)
        {
            if (manager.matchMaker == null)
            {
                manager.StartMatchMaker();
            }
            NetworkManager.singleton.networkPort = 7777;
            NetworkManager.singleton.StartClient(matchInfoData);
        }
        else
        {
            Debug.LogError("Join match failed");
        }
    }
}
