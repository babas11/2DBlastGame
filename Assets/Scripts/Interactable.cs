using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Interactable : MonoBehaviour
{

    SpriteRenderer spriteRenderer;
    enum InteractableType
    {
        box ,
        stone,
        vase
    }

    InteractableType type;
    

}
