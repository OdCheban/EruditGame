using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public static UIManager instance;

    [Header("UI:")]
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
    
    public void StartGame()
    {
        add_btn.gameObject.SetActive(true);
        rem_btn.gameObject.SetActive(true);
        points_text.gameObject.SetActive(true);
        stateGame_text.gameObject.SetActive(false);

        Player player = FindObjectOfType<Player>();
        rem_btn.onClick.AddListener(() => player.Disconnect());
        add_btn.onClick.AddListener(() => player.Connect());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MenuScene");
        }
    }
}
