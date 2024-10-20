public class Stone : Interactable, IObstacle
{
    public void Explode()
    {
        print("Stone Exploded");
    }

     override protected void OnMouseDown()
    {
        print("Cube Clicked");
    }
}

 