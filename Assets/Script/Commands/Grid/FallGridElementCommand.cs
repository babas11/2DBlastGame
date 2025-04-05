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
                FallToPositionWithJumpEffect(element.ElementTransfom,group.offScreenValue);
            }
        }
        
        private void FallToPositionWithJumpEffect(Transform elementTransform, float offScreenValue, float jumpHeight = 0.1f)
        {
            Vector3 targetPosition = elementTransform.position - new Vector3(0,offScreenValue, 0);
            Sequence seq = DOTween.Sequence();

            seq.Append(elementTransform.DOMove(targetPosition, _gridViewData.FallDuration).SetEase(Ease.Linear));

            float jumpDuration = _gridViewData.FallDuration/ 2f;
            seq.Append(elementTransform.DOMoveY(targetPosition.y + jumpHeight, jumpDuration * 0.5f)
                .SetEase(Ease.OutQuad));
            seq.Append(elementTransform.DOMoveY(targetPosition.y, jumpDuration * 0.5f)
                .SetEase(Ease.InQuad));
        }
    }
}