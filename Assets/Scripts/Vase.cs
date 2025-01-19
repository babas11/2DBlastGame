
using System.Collections;
using UnityEngine;

public class Vase : Interactable, IObstacle
{
    [SerializeField]
    private
    Sprite[] sprites;
    public Vector2Int ObstacleMatrixPos
    {
        get =>  this.MatrixPosition;
    }
    public Vector3 ObstacleWorldPos
    {
        get
        {
            return this.transform.position;
        }
    }

    public InteractableType ObstacleType 
    {
        get 
        {
            return this.Type; 
        } 
    }

    public Interactable InteractableObstacle
    {
        get { return this; }
    }
    public int Health { get; private set; } = 2;
    public override bool CanFall => true; // Vase falls down vertically


    public bool TakeDamage(int damage, BlastType blastType)
    {
        Health -= damage;
        if (Health > 0)
        {
            Health = Mathf.Max(Health, 0);
            StartCoroutine(CartoonishScaleToTarget(2.5f, 1.3f, 1f));
            ChangeSprite();
            return false;
        }
       
        return true;
    }



    void ChangeSprite()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[1];
    }

   

    public void UpdateObjectives()
    {
         uiController.UpdateObjectives(this);
    }

    public void KillObstacle()
    {
        throw new System.NotImplementedException();
    }
}
