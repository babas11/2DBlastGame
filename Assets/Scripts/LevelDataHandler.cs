using System.IO;
using UnityEngine;

public class LevelDataHandler : MonoBehaviour
{

    public LevelData levelData { get; private set; }

    [SerializeField]
    [Tooltip("Name of the file to read")]
    private int levelToLoad;
    public string LevelToLoadString(int level)
    {

        if (level < 10)
        {
            return $"level_0{level}.json";
        }
        else
        {
            return $"level_{level}.json";
        }

    }


    string SavePath => Path.Combine(Application.streamingAssetsPath, "Save.json");
    string DefaultPath => Path.Combine(Application.streamingAssetsPath, LevelToLoadString(1));


    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        ReadLevel();
    }



    void ReadLevel()
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

    

    public void ReadLevel(int levelNumber)
{
    string levelPath = Path.Combine(Application.streamingAssetsPath, LevelToLoadString(levelNumber));
    if (File.Exists(levelPath))
    {
        var fileContents = File.ReadAllText(levelPath);
        levelData = JsonUtility.FromJson<LevelData>(fileContents);
    }
    else
    {
        Debug.LogError($"Level file for level {levelNumber} not found.");
    }
}

    public void SaveLevel(LevelData newlevelData)
    {
        string json = JsonUtility.ToJson(newlevelData, true);
        File.WriteAllText(SavePath, json);
    }


}
