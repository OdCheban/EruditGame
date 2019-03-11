using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadData : MonoBehaviour {

	void Start ()
    {
        if (!DataGame.loadData)
        {
            DataGame.ReadDictonaryFromFile();
            DataGame.LoadData();
        }
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
