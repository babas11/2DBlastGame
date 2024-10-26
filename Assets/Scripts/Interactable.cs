using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class Interactable : Mover
{
    public readonly Dictionary<InteractableType, string> interactableStrings = new Dictionary<InteractableType, string>
    {
        {InteractableType.red, "r"},
        {InteractableType.green, "g"},
        {InteractableType.blue, "b"},
        {InteractableType.yellow, "y"},
        {InteractableType.box, "bo"},
        {InteractableType.tnt, "t"},
        {InteractableType.stone, "s"},
        {InteractableType.vase, "v"},
        {InteractableType.random, default}
    };
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

    public override string ToString()
    {
        return interactableStrings[type];
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
