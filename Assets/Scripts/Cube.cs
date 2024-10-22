using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Cube : Interactable
{
    
    
    override protected void OnMouseDown()
    {
        print(matrixPosition);
        if(this.idle && interactableGridSystem.LookForMatch(out  List<Interactable> matchList,this))
        {
            StartCoroutine(interactableGridSystem.Blast(matchList, this.transform));
        } else
        {
            // Scale up and down for visual eye cathing responsive effect on no match 
            StartCoroutine(CartoonishScaleToTarget(2.5f, 1.3f,1f));
        }
    }
    
}

 