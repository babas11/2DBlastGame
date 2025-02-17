using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class Box : Interactable, IObstacle
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
        //Decrease health by damage
        Health -= damage;

        //Limit health to 0
        Health = Mathf.Max(Health, 0);

        if(Health == 0)
        {
            //Destroy the object
            //UpdateObjectives();
            KillObstacle();
        }
        return true;
    }

    public void UpdateObjectives()
    {
        
    }

    public void KillObstacle()
    {
        ResetInteractable(interactablePool.transform);
    }
}
