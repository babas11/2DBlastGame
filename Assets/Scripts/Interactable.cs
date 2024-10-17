using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Interactable : MonoBehaviour
{

    SpriteRenderer spriteRenderer;

    public InteractableType type { get; set; }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    
}




public enum InteractableType
    {
        red,
        green ,
        blue,
        yellow,
        cube,
        box ,
        stone,
        random,
        vase
    }

 