using Sirenix.OdinInspector;
using UnityEngine;

namespace Script.Controllers.Obstacle
{
    public class ObstacleSpriteController : MonoBehaviour
    {
        #region Self Variables

        #region Serialized Variables

        [SerializeField] private SpriteRenderer spriteRenderer;

        #endregion
        
        #region Private Variables

        [ShowInInspector]private Sprite[] _obstacleSprites;
        private byte _currentSpriteIndex;
        
        #endregion
        
        #endregion

        internal void SetSpriteData(Sprite[] obstacleSprites)
        {
            _currentSpriteIndex = 0;
            _obstacleSprites = obstacleSprites;
            SetObstacleSprite();
        }

        private void SetObstacleSprite()
        {
            spriteRenderer.sprite = _obstacleSprites[_currentSpriteIndex];
        }
        
        internal void ChangeObstacleSpriteAfterDamage()
        {
            _currentSpriteIndex ++;
            SetObstacleSprite();
            
        }

        internal void SetSortingOrder(Vector2Int matrixPosition)
        {
            spriteRenderer.sortingOrder = matrixPosition.y + 1;
        }

    }
}