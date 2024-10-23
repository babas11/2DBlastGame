using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class Box : Interactable, IObstacle
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

    override protected void OnMouseDown()
    {
        print("Box Clicked");
    }

  
}

public interface IObstacle
{
    int Health { get; }
    void TakeDamage();
    void ResetHealth();
}
