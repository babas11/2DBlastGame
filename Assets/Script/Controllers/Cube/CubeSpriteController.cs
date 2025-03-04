using Script.Data.UnityObjects.Script.Data.UnityObjects;
using Script.Data.ValueObjects;
using Sirenix.OdinInspector;
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

        [ShowInInspector] private Sprite _tntSprite;
        [ShowInInspector] private Sprite _cubeSprite;
        [ShowInInspector] private Sprite _cubeTntSprite;
        
        #endregion

        #endregion

        internal void SetControllerData(CubeData cubeData,TntData tntData)
        {
            _tntSprite = tntData.Sprite;
            _cubeSprite = cubeData.cubeSprite;
            _cubeTntSprite = cubeData.cubeTntSprite;
            SetCubeSpriteOnDefault();
        }

        internal void SetCubeSpriteOnDefault()
        {
            spriteRenderer.sprite = _cubeSprite;
        }
        
        internal void SetCubeSpriteOnTnt()
        {
            spriteRenderer.sprite = _cubeTntSprite;
        }
        internal void SetTntSprite()
        {
            spriteRenderer.sprite = _tntSprite;
        }

        internal void SetSortingOrder(Vector2Int matrixPosition)
        {
            spriteRenderer.sortingOrder = matrixPosition.y;
        }


       
    }
}