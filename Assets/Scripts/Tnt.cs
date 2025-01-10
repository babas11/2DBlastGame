using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Tnt : Interactable,IExplosive
{
    Renderer rend;
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
    }
    public static event Action<Tnt,int> OnTntPressed;
    public override bool CanFall => true;
    public bool exploded = false;
    public int ExplosionArea { get; } = 2;

    public void Explode()
    {
        throw new System.NotImplementedException();
    }

    public void HideTnt()
    {
        rend.enabled = false;
    }


    protected override void OnMouseDown()
    {
        //base.OnMouseDown();
        OnTntPressed?.Invoke(this,ExplosionArea);
    }
}

