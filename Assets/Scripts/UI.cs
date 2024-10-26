
using System.Collections;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : Mover
{

    [SerializeField]
    GameObject celebrationStar;

    [SerializeField]
    GameObject popUpBackground;

    [SerializeField]
    GameObject celebrationEfect;
    [SerializeField]
    GameObject failCross;

    [SerializeField]
    GameObject uiObjectivePrefab;

    [SerializeField]
    Image topImage;

    [SerializeField]
    Sprite[] uiObjectiveSprites;

    [SerializeField]
    Transform gridLayoutTransform;
    int moves = 10;

    [SerializeField]
    TMP_Text movesText;

    GameObject boxUI;
    int totalBoxes;
    GameObject vaseUI;
    int totalVases;
    GameObject stoneUI;
    int totalStones;
    LevelDataHandler levelDataHandler;


    public int Moves { get => moves; }
    void Start()
    {
        levelDataHandler = FindObjectOfType<LevelDataHandler>();
        movesText.text = moves.ToString();
    }

    public void SetupUI(LevelData levelData)
    {
        moves = levelData.move_count;
        movesText.text = moves.ToString();
        FindObjectivesOfTheLevel(levelData);

    }

    IEnumerator EndScene(bool success = true){
        Vector3 offset = new Vector3(0, 10, 0);

        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane));

        Vector3 finalPos = new Vector3((topRight.x + bottomLeft.x) / 2, (topRight.y + bottomLeft.y) / 2 -1.5f, 0);
        Vector3 offsetPos = finalPos + offset;

        popUpBackground.transform.position = offsetPos;
        
        yield return popUpBackground.GetComponent<Mover>().MoveToPositionWithJump(finalPos, 3f, 1f);
        if (success)
        {
            celebrationStar.transform.position = offsetPos;
            yield return celebrationStar.GetComponent<Mover>().MoveToPositionWithJump(finalPos, 2.5f,1.3f);

            levelDataHandler.ReadLevel(levelDataHandler.levelData.level_number + 1);   
            
            yield return celebrationStar.GetComponent<Mover>().CartoonishScaleToTarget(1.2f, 1.4f, 1f);
        }
        else
        {
            failCross.transform.position = offsetPos;
            yield return failCross.GetComponent<Mover>().MoveToPositionWithJump(finalPos, 2.5f,1.3f);
            
            yield return failCross.GetComponent<Mover>().CartoonishScaleToTarget(1.2f, 1.4f, 1f);
            levelDataHandler.ReadLevel(levelDataHandler.levelData.level_number +1);   
        }

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(0);
    }

    public void DecreaseMoves()
    {
        moves--;
        if (moves < 0)
        {
            moves = 0;
            StartCoroutine(EndScene(false));

        }
        movesText.text = moves.ToString();
    }
    void FindObjectivesOfTheLevel(LevelData levelData)
    {
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


        totalVases = vaseCounter;

        if (boxCounter > 0)
        {
            totalBoxes = boxCounter;
            boxUI = Instantiate(uiObjectivePrefab, gridLayoutTransform);
            boxUI.GetComponent<Image>().sprite = uiObjectiveSprites[0];
            boxUI.GetComponentInChildren<TMP_Text>().text = boxCounter.ToString();
        }
        if (stoneCounter > 0)
        {
            totalStones = stoneCounter;
            stoneUI = Instantiate(uiObjectivePrefab, gridLayoutTransform);
            stoneUI.GetComponent<Image>().sprite = uiObjectiveSprites[1];
            stoneUI.GetComponentInChildren<TMP_Text>().text = stoneCounter.ToString();
        }
        if (vaseCounter > 0)
        {
            totalVases = vaseCounter;
            vaseUI = Instantiate(uiObjectivePrefab, gridLayoutTransform);
            vaseUI.GetComponent<Image>().sprite = uiObjectiveSprites[2];
            vaseUI.GetComponentInChildren<TMP_Text>().text = vaseCounter.ToString();
        }

    }

    public void UpdateObjectives(IObstacle type)
    {

        if (type is Box)
        {
            totalBoxes--;
            if (totalBoxes > 0)
            {
                boxUI.GetComponentInChildren<TMP_Text>().text = (int.Parse(boxUI.GetComponentInChildren<TMP_Text>().text) - 1).ToString();
            }
            else
            {
                boxUI.GetComponentInChildren<TMP_Text>().text = "";
                foreach (var image in boxUI.GetComponentsInChildren<Image>())
                {
                    image.gameObject.SetActive(false);
                }
            }
        }
        if (type is Stone)
        {
            totalStones--;
            if (totalStones > 0)
            {
                stoneUI.GetComponentInChildren<TMP_Text>().text = (int.Parse(stoneUI.GetComponentInChildren<TMP_Text>().text) - 1).ToString();
            }
            else
            {
                stoneUI.GetComponentInChildren<TMP_Text>().text = "";
                foreach (var image in stoneUI.GetComponentsInChildren<Image>())
                {
                    image.gameObject.SetActive(false);
                }
            }
        }
        if (type is Vase)
        {
            totalVases--;
            if (totalStones > 0)
            {
                vaseUI.GetComponentInChildren<TMP_Text>().text = (int.Parse(vaseUI.GetComponentInChildren<TMP_Text>().text) - 1).ToString();
            }
            else
            {
                vaseUI.GetComponentInChildren<TMP_Text>().text = "";
                foreach (var image in vaseUI.GetComponentsInChildren<Image>())
                {
                    image.gameObject.SetActive(false);
                }
            }
        }

        if (totalBoxes == 0 && totalStones == 0 && totalVases == 0)
        {
            StartCoroutine(EndScene(true));
        }



    }
}
