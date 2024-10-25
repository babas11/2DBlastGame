using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Cube : Interactable
{
    public enum CubeState
    {
        Default,
        TNT
    }

    public override bool CanFall => true;

    [SerializeField]
    CubeState tntState = CubeState.Default;
    public CubeState TNTState
    {
        get => tntState;
        set
        {
            if (value == CubeState.TNT)
            {
                tntState = value;
                this.GetComponent<SpriteRenderer>().sprite = cubeSprites[1];
            }
            else if (value == CubeState.Default)
            {
                tntState = value;
                this.GetComponent<SpriteRenderer>().sprite = cubeSprites[0];
            }

        }
    } 

    [SerializeField] Sprite[] cubeSprites;
    public Sprite[] Sprites { get => cubeSprites; set => cubeSprites = value; }


    override protected void OnMouseDown()
    {
        if (this.idle && interactableGridSystem.LookInteractableForMatchingAdjacent(out List<Interactable> matchList, this))
        {
            base.OnMouseDown();
            StartCoroutine(interactableGridSystem.Blast(matchList, this.transform));
        }
        else
        {
            // Scale up and down for visual eye cathing responsive effect on no match 
            StartCoroutine(CartoonishScaleToTarget(2.5f, 1.3f, 1f));
        }
    }

}



