using UnityEngine;

namespace Script.Controllers.Cube
{
    public class CubeSpriteController : MonoBehaviour
    {
        #region Self Variables

        #region Serialized Variables

        [SerializeField] private SpriteRenderer spriteRenderer;

        #endregion

        #region Private Variables

        private Sprite _cubeSprite;
        private Sprite _cubeTntSprite;

        #endregion

        #endregion

        internal void SetCubeSpriteOnDefault(Sprite cubeSprite)
        {
            spriteRenderer.sprite = cubeSprite;
        }
        
        internal void SetCubeSpriteOnTnt(Sprite cubeTntSprite)
        {
            spriteRenderer.sprite = cubeTntSprite;
        }

        internal void SetSortingOrder(Vector2Int matrixPosition)
        {
            spriteRenderer.sortingOrder = matrixPosition.y;
        }
     
        
    }
}