using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.EditorTools;
using UnityEngine;

public class LevelDataHandler : MonoBehaviour
{
    public LevelData levelData { get; private set; }

    [SerializeField]
    [Tooltip("Name of the file to read")]
    private int levelToLoad;
    public string levelToLoadString
    {
        get
        {
            if (levelToLoad < 10)
            {
                return $"level_0{levelToLoad}.json";
            }
            else
            {
                return $"level_{levelToLoad}.json";
            }
        }
    }

    string SavePath => Path.Combine(Application.streamingAssetsPath, "Save.json");

    private string jsonFilePath;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        jsonFilePath = jsonFilePath = Path.Combine(Application.streamingAssetsPath, levelToLoadString);
        ReadGrid(jsonFilePath);
    }



    void ReadGrid(string path)
    {

        if (File.Exists(SavePath))
        {
            print(SavePath);

        }
        else
        {
            if (!File.Exists(path))
            {
                Debug.LogError("File not found");
            }
            path = Path.Combine(Application.streamingAssetsPath, levelToLoadString);
        }
        var fileContents = File.ReadAllText(path);
        levelData = JsonUtility.FromJson<LevelData>(fileContents);

    }

    public void SaveLevel()
    {
        jsonFilePath = jsonFilePath = Path.Combine(Application.streamingAssetsPath, "Save.json");
        string json = JsonUtility.ToJson(levelData, true);
        File.WriteAllText(jsonFilePath, json);
    }


}
