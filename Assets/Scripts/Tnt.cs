using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tnt : Interactable
{

    [SerializeField]
    public override bool CanFall => true;
    public bool Exploded = false;



    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        StartCoroutine(Explode());
    }

    public IEnumerator Explode(bool chainExplosion = false)
    {
        HashSet<Tnt> tntsInCombo = new HashSet<Tnt> { this };
        
        // Check for adjacent TNTs to form a combo
        
        if(!chainExplosion){
            interactableGridSystem.LookForMatchingsOnAxis<Tnt>(out tntsInCombo,this);
            tntsInCombo.Add(this);
        }
        //Enhancing the range of explotion whether it is a combo or not
        int blastRange = tntsInCombo.Count > 1 ? 3 : 2;

        StartCoroutine(interactableGridSystem.TNTBlast(this,blastRange));

        yield return null;

    }

}

