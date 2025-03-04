using DG.Tweening;
using Script.Data.ValueObjects;
using Script.Interfaces;
using Script.Keys;
using UnityEngine;

namespace Script.Commands.Grid
{
    public class FallGridElementCommand
    {
        private GridViewData _gridViewData;
        public FallGridElementCommand(GridViewData gridViewData)
        {
            _gridViewData = gridViewData;
        }

        internal void Execute(FallingElementGroup group)
        {
           
            foreach (IGridElement element in group.elementsToFall)
            {
                Fall(element.ElementTransfom,group.offScreenValue);
            }
        }

        private void Fall(Transform elementTransform,float offScreenValue)
        {
            Vector3 targetPos = elementTransform.position - new Vector3(0,offScreenValue, 0);
            elementTransform.DOMove(targetPos, _gridViewData.FallDuration).SetEase(Ease.Linear);
        }
    }
}