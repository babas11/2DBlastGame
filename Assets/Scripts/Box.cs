public class Box :  Interactable, IObstacle
{
    public void Explode()
    {
        print("Box Exploded" + this.matrixPosition.x + " , " + this.matrixPosition.y);
    }

    override protected void OnMouseDown()
    {
        print("Box Clicked");
    }
}
