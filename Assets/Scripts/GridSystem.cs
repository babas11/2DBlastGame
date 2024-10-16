using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    private void Awake()
    {
        // Read grid's attributes and initialize its properties
        BuildGrid();

        
    }

    //Testing pupose
    [SerializeField]
    Vector2Int dimensions = new Vector2Int(9,10);
    
    Interactable[,] matrix;


    void BuildGrid()
    {
        
        string testString = "";
        var stringInteractables = ReadGrid();

        for (int y = 0 ; y != dimensions.y -1 ; y++)
        {
            for (int x = 0; x != dimensions.x - 1; x++)
            {
                testString += $"({x},{y}) ";

            }
            testString += "\n";
        }
        print(testString);

    }

    string[] ReadGrid()
    {
        JsonRepository repository = new JsonRepository();
        return repository.Read("pathFile");
    }


}
