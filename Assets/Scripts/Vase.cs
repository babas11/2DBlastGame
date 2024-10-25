
using System.Collections;
using UnityEngine;

public class Vase : Interactable, IObstacle
{
    private UI ui;
    private void Start()
    {
        ui = FindObjectOfType<UI>();
    }
    [SerializeField]
    private
    Sprite[] sprites;
    public int Health { get; private set; } = 2;
    public override bool CanFall => true; // Vase falls down vertically


    public void TakeDamage(int damage, bool isTNTBlast)
    {
        // Normal blast, Vase can take multiple damages
        Health -= damage;
        Health = Mathf.Max(Health, 0);
        StartCoroutine(CartoonishScaleToTarget(2.5f, 1.3f, 1f));
        ChangeSprite();

        if (Health == 0)
        {
            UpdateObjectives();
        }
    }



    void ChangeSprite()
    {
        if (Health > 1)
        {

        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = sprites[1];
        }

    }

   

    public void UpdateObjectives()
    {
         ui.UpdateObjectives(this);
    }

}
