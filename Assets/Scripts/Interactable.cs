using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class Interactable : MonoBehaviour
{

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
}
