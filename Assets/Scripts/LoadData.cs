using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadData : MonoBehaviour {

	void Start ()
    {
        DataGame.ReadDictonaryFromFile();
        DataGame.LoadData();
        SceneManager.LoadScene("Menu");
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene("preMenu");
        }
    }
}
