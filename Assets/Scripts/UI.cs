using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField]
    GameObject uiObjectivePrefab;

    [SerializeField]
    Sprite[] uiObjectiveSprites;

    [SerializeField]
    Transform gridLayoutTransform;
    int moves = 10;

    [SerializeField]
    TMP_Text movesText;

    GameObject boxUI;
    GameObject vaseUI;
    GameObject stoneUI;


    public int Moves { get => moves;}
    void Start()
    {
        movesText.text = moves.ToString();
    }

    public void SetupUI(LevelData levelData){
        moves = levelData.move_count;
        movesText.text = moves.ToString();
        FindObjectivesOfTheLevel(levelData);

    }

    public void DecreaseMoves()
    {
        moves--;
        movesText.text = moves.ToString();
    }
    void FindObjectivesOfTheLevel(LevelData levelData){
        int boxCounter = 0;
        int vaseCounter = 0;
        int stoneCounter = 0;
        foreach (var type in levelData.grid)
        {
            if (type == "bo")
            {
                boxCounter++;
            }
            if (type == "s")
            {
                stoneCounter++;
            }
            if (type == "v")
            {
                vaseCounter++;
            }
        }

        if (boxCounter > 0)
        {
            boxUI = Instantiate(uiObjectivePrefab, gridLayoutTransform);
            boxUI.GetComponent<Image>().sprite = uiObjectiveSprites[0];
            boxUI.GetComponentInChildren<TMP_Text>().text = boxCounter.ToString();
        }
        if (stoneCounter > 0)
        {
            stoneUI = Instantiate(uiObjectivePrefab, gridLayoutTransform);
            stoneUI.GetComponent<Image>().sprite = uiObjectiveSprites[1];
            stoneUI.GetComponentInChildren<TMP_Text>().text = stoneCounter.ToString();
        }
        if (vaseCounter > 0)
        {
            vaseUI = Instantiate(uiObjectivePrefab, gridLayoutTransform);
            vaseUI.GetComponent<Image>().sprite = uiObjectiveSprites[2];
            vaseUI.GetComponentInChildren<TMP_Text>().text = vaseCounter.ToString();
        }

    }

    public void  UpdateObjectives(IObstacle type){
        if (type is Box)
        {
            boxUI.GetComponentInChildren<TMP_Text>().text = (int.Parse(boxUI.GetComponentInChildren<TMP_Text>().text) - 1).ToString();
        }
        if (type is Stone)
        {
            stoneUI.GetComponentInChildren<TMP_Text>().text = (int.Parse(stoneUI.GetComponentInChildren<TMP_Text>().text) - 1).ToString();
        }
        if (type is Vase)
        {
            vaseUI.GetComponentInChildren<TMP_Text>().text = (int.Parse(vaseUI.GetComponentInChildren<TMP_Text>().text) - 1).ToString();
        }



    }
}
