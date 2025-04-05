using Script.Data.ValueObjects;
using Script.Managers;
using UnityEngine;

namespace Script.Commands.Grid
{
    public class PlaceGridCommand
    {
        private GridManager _manager;
        private CustomGridData _data;
        private GridViewData _gridViewData;
        
        public PlaceGridCommand(GridManager manager, CustomGridData data, GridViewData gridViewData)
        {
            _gridViewData = gridViewData;
            _manager = manager;
            _data = data;
        }

        internal void Execute()
        {
            Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
            Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane));
            float worldWidth = topRight.x - bottomLeft.x;
            float gridXPosition = (worldWidth - (_data.grid_width * _gridViewData.GridUnit)) / 2f;
            float gridYPosition =  _gridViewData.GridBottomOffset;
            _manager.transform.position = bottomLeft + new Vector3(gridXPosition + _gridViewData.GridUnit/2, gridYPosition, 0f);
        }
        
    }
}