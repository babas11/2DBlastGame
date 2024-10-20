using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Cube : Interactable
{
    
    
    override protected void OnMouseDown()
    {
        //print(this.matrixPosition);
        if(this.idle && interactableGridSystem.LookForMatch(out  List<Interactable> matchList,this))
        {
            StartCoroutine(interactableGridSystem.Blast(matchList, this.transform));
            //print("Match Found");
        }
    }
    
}

 