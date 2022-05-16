using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public string inputtedName;

    public InputField IF_Box;
    public Text BestScore;

    private void Awake()
    {
        instance = this;

        GameObject[] managers = GameObject.FindGameObjectsWithTag("Manager");
        
        if (managers.Length > 1)
        {
            Destroy(gameObject);
        } else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        //File.Delete(Application.persistentDataPath + "/savefile.json");
        LoadData();
    }

    public void SetName()
    {
        inputtedName = IF_Box.text;
        SceneManager.LoadScene(1);
    }

    public void LoadData()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            MainManager.PlayerData data = JsonUtility.FromJson<MainManager.PlayerData>(json);

            BestScore.text = "Best Score: " + data.name + " " + data.bestScore.ToString();
        }
    }
}
