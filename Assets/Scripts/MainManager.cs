using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public static MainManager instance;

    public Text ScoreText;
    public Text HighScoreText;
    public GameObject GameOverText;
    
    private bool m_Started = false;
    private int m_Points;
    
    private bool m_GameOver = false;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        HighScoreText.text = QuickLoad().name + " " + QuickLoad().bestScore.ToString();
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        SaveData();
        GameOverText.SetActive(true);

    }

    [System.Serializable]
    public class PlayerData
    {
        public int bestScore;
        public string name;
    }

    public void SaveData()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        PlayerData data = new PlayerData();
        data.bestScore = m_Points;
        if (UIManager.instance.inputtedName == null)
        {
            UIManager.instance.inputtedName = "Unknown";
        }
        data.name = UIManager.instance.inputtedName;
        Debug.Log(data.name);
        Debug.Log(data.name.Length);

        if (!File.Exists(path))
        {
            if (data.name.Length < 1)
            {
                data.name = "Unknown";
            }
            string json = JsonUtility.ToJson(data);
            
            File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
        } else
        {
            PlayerData oldData = QuickLoad();
            if (data.bestScore > oldData.bestScore)
            {
                if (data.name.Length < 1)
                {
                    data.name = "Unknown";
                }
                string json = JsonUtility.ToJson(data);
                
                File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
            }
        }
    }

    public PlayerData QuickLoad()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);

            return data;
        }
        return null;
    }
}
