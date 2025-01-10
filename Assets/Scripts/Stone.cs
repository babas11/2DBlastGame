using System.Collections;
using UnityEngine;


public class Stone : Interactable, IObstacle
{
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
    public Interactable InteractableObstacle
    {
        get { return this; }
    }

    public InteractableType ObstacleType 
    {
        get 
        {
            return this.Type; 
        } 
    }
    public int Health { get; private set; } = 1;
    public override bool CanFall => false;

    public bool TakeDamage(int damage, BlastType blastType)
    {
        if (blastType != BlastType.Regular)
        {
            //Decrease health by damage
            Health -= damage;
            //Limit health to 0
            Health = Mathf.Max(Health, 0);
            if (Health == 0)
            {
                UpdateObjectives();
            }
            return true;
        }
        return false;
    }

    public void UpdateObjectives()
    {
         //uiController.UpdateObjectives(this);
    }

    public void KillObstacle()
    {
        ScaleUpAndDown(1.1f,1.0f,0.1f,true);
        
    }
}

 