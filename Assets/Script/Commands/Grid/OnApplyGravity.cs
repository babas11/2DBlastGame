using System;
using DG.Tweening;
using Script.Data.ValueObjects;
using Script.Extensions;
using Script.Interfaces;
using Script.Managers;
using Script.Signals;
using Script.Utilities.Grid;
using UnityEngine;

namespace Script.Commands.Grid
{
    public class OnApplyGravity
    {
        private GridViewData _gridViewData;
        private Vector2Int _dimensions;
        private GridManipulationUtilities<IGridElement> _gridManipulationUtilities;
        public OnApplyGravity(Vector2Int dimensions, GridManipulationUtilities<IGridElement> gridManipulationUtilities, GridViewData gridViewData)
        {
            _dimensions = dimensions;
            _gridManipulationUtilities = gridManipulationUtilities;
            _gridViewData = gridViewData;
        }

        internal void Execute()
        {
            for (int x = 0; x < _dimensions.x; x++)
            {
                for (int y = 0; y < _dimensions.y - 1; y++)
                {
                    if (_gridManipulationUtilities.IsEmpty(x, y))
                    {
                        for (int yAbove = y + 1; yAbove < _dimensions.y; yAbove++)
                        {
                            if (!_gridManipulationUtilities.IsEmpty(x, yAbove))
                            {
                                var interactable = _gridManipulationUtilities.GetItemAt(x, yAbove);
                                bool canFall = true;

                                if (interactable.Type.IsObstacle())
                                {
                                    canFall = interactable.CanFall;
                                }

                                if (canFall)
                                {
                                    MoveInteractable(interactable,x,y,yAbove);
                                    break;
                                }
                                else
                                {
                                    break;
                                }
                                
                            }
                        }
                    }
                }
            }
            GridSignals.Instance.onSetCubeState?.Invoke();
        }
        private void MoveInteractable(IGridElement interactable, int x, int y, int yNotEmpty)
        {
            _gridManipulationUtilities.RemoveItemAt(x, yNotEmpty);
            _gridManipulationUtilities.PutItemAt(x, y, interactable);
            interactable.SetMetrixPosition(new Vector2Int(x, y));
            FallToPositionWithJumpEffect(interactable.ElementTransfom,_gridManipulationUtilities.GridPositionToWorldPosition(x, y), null, 0.1f);
        }
        private  void MoveTo(IGridElement element,Vector3 targetPosition, float duration, Ease ease = Ease.Linear, Action onComplete = null)
        {
            Tween tween = element.ElementTransfom.DOMove(targetPosition, duration).SetEase(ease);
            tween.OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }
        private void FallToPositionWithJumpEffect(Transform elementTransform,Vector3 targetPosition, Action onComplete = null, float jumpHeight = 0.1f)
        {
            Sequence seq = DOTween.Sequence();

            seq.Append(elementTransform.DOMove(targetPosition, _gridViewData.FallDuration/2f).SetEase(Ease.Linear));

            float jumpDuration = _gridViewData.FallDuration/ 3f;
            seq.Append(elementTransform.DOMoveY(targetPosition.y + jumpHeight, jumpDuration * 0.5f)
                .SetEase(Ease.OutQuad));
            seq.Append(elementTransform.DOMoveY(targetPosition.y, jumpDuration * 0.5f)
                .SetEase(Ease.InQuad));
        }
    }
}