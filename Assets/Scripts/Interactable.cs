using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class Interactable : Mover
{
    void Awake()
    {
        uiController = GameObject.FindObjectOfType<UI>();
    }

    public virtual bool CanFall => true;

    SpriteRenderer spriteRenderer;
    
    [SerializeField]
    private InteractableType type;

    UI uiController;

  
    
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
        uiController.DecreaseMoves();
    }

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
