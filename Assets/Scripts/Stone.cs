using System.Collections;
using UnityEngine;


public class Stone : Interactable, IObstacle
{

    public int Health { get; private set; } = 1;
    public override bool CanFall => false;

    public void TakeDamage(int damage, bool isTNTBlast)
    {
        if (isTNTBlast)
        {
            //Decrease health by damage
            Health -= damage;
            //Limit health to 0
            Health = Mathf.Max(Health, 0);
            //Play animation to visual feedback
            StartCoroutine(CartoonishScaleToTarget(2.5f, 1.3f, 1f));
        }
    }
  
}

 