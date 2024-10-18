public class Cube : Interactable
{
    
    override protected void OnMouseDown()
    {
        print(this.matrixPosition);
        interactableGridSystem.LookForMatch(this);
    }
    
}

 