﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : NetworkBehaviour {
    public static UIManager instance;
    public NetworkManager manager;

    [Header("UI:")]
    public GameObject player_ui;
    public GameObject worldRoom_ui;
    public GameObject room_ui;
    public Text roomInfo;
    public Text roomChat;

    public Button add_btn;
    public Button rem_btn;
    [HideInInspector] public Image rem_img;
    [HideInInspector] public Image add_img;
    public Text points_text;
    public Text stateGame_text;

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

    [ClientRpc]
    public void RpcRoomInfo(int k,int maxK)
    {
        roomInfo.text = k + "/" + maxK + " Название_комнаты ";
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
        manager.StartHost();
        EnterRoom();
    }
    public void ConnectRoom()
    {
        manager.StartClient();
        EnterRoom();
    }

    public void LeaveRoom()
    {
        Camera.main.transform.position = new Vector3(0, 0, -10);
        worldRoom_ui.SetActive(true);
        room_ui.SetActive(false);
        player_ui.SetActive(false);
    }
    void EnterRoom()
    {
        Camera.main.transform.position = new Vector3(4, 0, -10);
        worldRoom_ui.SetActive(false);
        room_ui.SetActive(true);
        player_ui.SetActive(false);
    }


}
