
using System.Collections;
using UnityEngine;

public class Vase : Interactable, IObstacle
{
    [SerializeField] private 
    Sprite[] sprites;
    public int Health { get; private set; } = 2;
    public override bool CanFall => true; // Vase falls down vertically

    private bool hasTakenDamageFromCurrentTNTBlast = false;

    public void TakeDamage(int damage, bool isTNTBlast)
    {
        if (isTNTBlast)
        {
            if (!hasTakenDamageFromCurrentTNTBlast)
            {
                Health -= damage;
                Health = Mathf.Max(Health, 0);
                hasTakenDamageFromCurrentTNTBlast = true;
                StartCoroutine(CartoonishScaleToTarget(2.5f, 1.3f, 1f));
            }
        }
        else
        {
            // Normal blast, Vase can take multiple damages
            Health -= damage;
            Health = Mathf.Max(Health, 0);
            StartCoroutine(CartoonishScaleToTarget(2.5f, 1.3f, 1f));
        }
        ChangeSprite();
    }

    void ChangeSprite()
    {
        if(Health > 1)
        {
            
        }else
        {
            GetComponent<SpriteRenderer>().sprite = sprites[1];
        }
        
    }

    public void ResetDamageFlag()
    {
        hasTakenDamageFromCurrentTNTBlast = false;
    }

}
