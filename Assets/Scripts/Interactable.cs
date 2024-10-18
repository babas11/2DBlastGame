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

    void OnPointerDown()
    {
        print("Clicked");
    }

    
}




public enum InteractableType
    {
        red,
        green ,
        blue,
        yellow,
        box ,
        stone,
        tnt,
        random,
        vase
    }

 