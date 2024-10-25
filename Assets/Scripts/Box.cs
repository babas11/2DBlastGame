using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class Box : Interactable, IObstacle
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
        //Decrease health by damage
        Health -= damage;

        //Limit health to 0
        Health = Mathf.Max(Health, 0);

        if(Health == 0)
        {
            //Destroy the object
            UpdateObjectives();
        }
    }

    public void UpdateObjectives()
    {
        ui.UpdateObjectives(this);
    }
}


public interface IObstacle
{
    int Health { get; }
    void TakeDamage(int damage, bool isTNTBlast);

    void UpdateObjectives();

}
