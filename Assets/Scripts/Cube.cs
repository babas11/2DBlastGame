using System.Collections.Generic;
using UnityEngine;

public class Cube : Interactable
{
    public enum CubeState
    {
        Default,
        Tnt
    }

    public override bool CanFall => true;

    [SerializeField]
    CubeState tntState = CubeState.Default;
    public CubeState TntState
    {
        get => tntState;
        set
        {
            if (value == CubeState.Tnt)
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

    public override void ResetInteractable(Transform parent)
    {
        base.ResetInteractable(parent);
        this.GetComponent<SpriteRenderer>().sprite = cubeSprites[0];
    }

    /*override protected void OnMouseDown()
    {
        if (this.idle && interactableGridSystem.LookInteractableForMatchingAdjacent(out List<Interactable> matchList, this))
        {
            base.OnMouseDown();
            interactableGridSystem.Blast(matchList, this.transform);
        }
        else
        {
            // Scale up and down for visual eye cathing responsive effect on no match 
            StartCoroutine(CartoonishScaleToTarget(2.5f, 1.3f, 1f));
        }
    }*/


}



