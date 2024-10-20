using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

public class Cube : Interactable
{
    
    override protected void OnMouseDown()
    {
        //print(this.matrixPosition);
        if(this.idle && interactableGridSystem.LookForMatch(out  List<Interactable> matchList,this))
        {
            interactableGridSystem.Blast(matchList, this.transform);
            print("Match Found");
        }
    }
    
}

 