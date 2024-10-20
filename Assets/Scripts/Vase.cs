public class Vase : Interactable
{
    public void Explode()
    {
        print("Vase Exploded");
    }

    override protected void OnMouseDown()
    {
        print("Vase Clicked");
    }
}
