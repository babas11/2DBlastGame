using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField]
    int moves = 10;

    [SerializeField]
    TMP_Text movesText;


    public int Moves { get => moves;}
    void Start()
    {
        movesText.text = moves.ToString();
    }

    public void UISetup(){

    }

    public void DecreaseMoves()
    {
        moves--;
        movesText.text = moves.ToString();
    }
}
