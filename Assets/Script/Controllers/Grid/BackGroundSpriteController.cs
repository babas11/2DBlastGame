using Script.Data.UnityObjects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Script.Controllers.Grid
{
    public class BackGroundSpriteController : MonoBehaviour
    {
        #region Self Variables
        
        #region Serialized Variables

        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private SpriteMask _spriteMask;

        #endregion
        
        #region Private Variables

        [ShowInInspector] private CD_Grid _data;
        private Vector2Int _dimensions;
        

        #endregion
        
        #endregion
        
        public void SetRendererData(CD_Grid data,Vector2Int dimensions)
        {
            _data = data;
            _dimensions = dimensions;
            
        }
        
        
        public void SetGridBackGroundSprite()
        {
            _renderer.sortingOrder = -1;
            _renderer.sprite = _data.GridViewData.GridBackground;
            float overScale = _data.GridViewData.BackgroundOverScale;
            float gridUnit = _data.GridViewData.GridUnit;
            float sizeDifference = _data.GridViewData.InteractableSizeDifference;
            
            float widthCenter = transform.position.x + _dimensions.x * gridUnit / 2f - gridUnit/2f;
            float heightCenter = transform.position.y + _dimensions.y * gridUnit / 2f - gridUnit/2f ;
            
            Vector3 center = new Vector3(widthCenter, heightCenter, 0);
            _renderer.transform.position = center;
            
            _renderer.size = new Vector2(_dimensions.x * gridUnit * overScale , _dimensions.y * overScale * gridUnit + sizeDifference );
        }
    }
}