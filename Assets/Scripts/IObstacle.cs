using UnityEngine;
public interface IObstacle 
{
    public Vector2Int ObstacleMatrixPos
    {
        get;
    }
    public Vector3 ObstacleWorldPos
    {
        get;
    }
    
    public InteractableType ObstacleType
    {
        get;
    }
    
    public Interactable InteractableObstacle
    {
        get;
    }
    
    int Health { get; }
    bool TakeDamage(int damage, BlastType blastType);

    void UpdateObjectives();
    
    void KillObstacle();

}
