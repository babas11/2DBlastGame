using UnityEngine;
using UnityEngine.UI;

public class ResizeGridItems : MonoBehaviour
{
    public GridLayoutGroup gridLayoutGroup;
    public RectTransform container; 
    public int padding = 10; 


    void AdjustItemSize()
    {
        float containerWidth = container.rect.width;
        
        float spacing = gridLayoutGroup.spacing.x;

        int columns = Mathf.FloorToInt((containerWidth + spacing) / (gridLayoutGroup.cellSize.x + spacing));

        if (columns > 0) // Prevent division by zero
        {
            float newWidth = (containerWidth - ((columns - 1) * spacing) - (gridLayoutGroup.padding.left + gridLayoutGroup.padding.right)) / columns;
            gridLayoutGroup.cellSize = new Vector2(newWidth - padding, gridLayoutGroup.cellSize.y);
        }
    }
}
