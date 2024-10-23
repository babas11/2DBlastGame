using System.Collections;
using UnityEngine;


public class Stone : Interactable, IObstacle
{
     public int Health{ get => health ; }
    
    [SerializeField]
    private int health;
    private int defaultHealth = 2;

    void OnEnable()
    {
        health = defaultHealth;
    }

    
    void Start()
    {
        health = defaultHealth;
    }
   
    
    public void TakeDamage(){
        health--;
        StartCoroutine(CartoonishScaleToTarget(2.5f, 1.3f, 1f));
    }

    public void ResetHealth(){
        health = defaultHealth;
    }   
}

 