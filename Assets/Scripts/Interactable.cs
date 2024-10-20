using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class Interactable : Mover
{
    public readonly static Dictionary<string, InteractableType> interactableStringTypes = new Dictionary<string, InteractableType>
        {
            {"r", InteractableType.red},
            {"g", InteractableType.green},
            {"b", InteractableType.blue},
            {"y", InteractableType.yellow},
            {"bo", InteractableType.box},
            {"t", InteractableType.tnt},
            {"s", InteractableType.stone},
            {"v", InteractableType.vase},
        };

    SpriteRenderer spriteRenderer;
    
    [SerializeField]
    private InteractableType type;

  
    
    [SerializeField]
    public Vector2Int matrixPosition;
    public InteractableType Type { get=> type; }
    protected InteractableGridSystem interactableGridSystem;
 
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        interactableGridSystem = FindObjectOfType<InteractableGridSystem>();
    }

    protected virtual void OnMouseDown()
    {
        print("Clicked");
    }

}

public interface IObstacle
{
    void Explode();
}



public enum InteractableType
{
    red,
    green,
    blue,
    yellow,
    box,
    tnt,
    stone,
    vase,
    random
}
