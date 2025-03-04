using System;
using DG.Tweening;
using Script.Extensions;
using Script.Interfaces;
using Script.Managers;
using Script.Utilities.Grid;
using UnityEngine;

namespace Script.Commands.Grid
{
    public class GridFallCommand
    {
        private Vector2Int _dimensions;
        private GridManipulationUtilities<IGridElement> _gridManipulationUtilities;
        public GridFallCommand(Vector2Int dimensions, GridManipulationUtilities<IGridElement> gridManipulationUtilities)
        {
            _dimensions = dimensions;
            _gridManipulationUtilities = gridManipulationUtilities;
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
                                bool canFall = interactable.CanFall;
                                if (canFall)
                                {
                                    MoveInteractable(interactable, x, y, yAbove);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        private void MoveInteractable(IGridElement interactable, int x, int y, int yNotEmpty)
        {
            _gridManipulationUtilities.RemoveItemAt(interactable.MatrixPosition.x, interactable.MatrixPosition.y);
            _gridManipulationUtilities.PutItemAt(x, y, interactable);
            interactable.SetMetrixPosition(new Vector2Int(x, y));
            MoveTo(interactable,_gridManipulationUtilities.GridPositionToWorldPosition(x, y), 0.3f);
        }
        private  void MoveTo(IGridElement element,Vector3 targetPosition, float duration, Ease ease = Ease.Linear, Action onComplete = null)
        {
            Tween tween = element.ElementTransfom.DOMove(targetPosition, duration).SetEase(ease);
            tween.OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }
    }
}