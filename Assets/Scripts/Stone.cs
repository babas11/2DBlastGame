using System.Collections;
using UnityEngine;


public class Stone : Interactable, IObstacle
{
    private UI ui;
    private void Start()
    {
        ui = FindObjectOfType<UI>();
    }
    public int Health { get; private set; } = 1;
    public override bool CanFall => false;

    public void TakeDamage(int damage, bool isTNTBlast)
    {
        if (isTNTBlast)
        {
            //Decrease health by damage
            Health -= damage;
            //Limit health to 0
            Health = Mathf.Max(Health, 0);
        }
        if (Health == 0)
        {
            UpdateObjectives();
        }
    }

    public void UpdateObjectives()
    {
         ui.UpdateObjectives(this);
    }
}

 