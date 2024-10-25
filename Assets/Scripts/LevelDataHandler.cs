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
    string DefaultPath => Path.Combine(Application.streamingAssetsPath, levelToLoadString);


    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        ReadGrid();
    }



    void ReadGrid()
    {

        if (File.Exists(SavePath))
        {
            var savedContent = File.ReadAllText(SavePath);
            levelData = JsonUtility.FromJson<LevelData>(savedContent);

        }
        else
        {
            var fileContents = File.ReadAllText(DefaultPath);
            levelData = JsonUtility.FromJson<LevelData>(fileContents);

        }


    }

    public void SaveLevel(LevelData newlevelData)
    {
        string json = JsonUtility.ToJson(newlevelData, true);
        File.WriteAllText(DefaultPath, json);
    }


}
