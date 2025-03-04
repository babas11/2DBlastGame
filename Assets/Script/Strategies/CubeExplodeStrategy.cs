using System;
using System.Collections.Generic;
using DG.Tweening;
using Script.Enums;
using Script.Extensions;
using Script.Interfaces;
using Script.Signals;
using Script.Utilities.Grid;
using UnityEngine;

namespace Script.Strategies
{
    public class CubeExplodeStrategy: IExplodeStrategy
    {
        private GridManipulationUtilities<IGridElement> _gridUtils;
        private GridFinder _gridFinder;
        private byte _minimumAmountToMakeTnt;

        public CubeExplodeStrategy(GridManipulationUtilities<IGridElement> gridUtils, GridFinder gridFinder)
        {
            _gridUtils = gridUtils;
            _minimumAmountToMakeTnt = 5;
            _gridFinder = gridFinder;
        }
        public void Explode(IGridElement element)
        {
            if (_gridFinder.LookInteractableForMatchingAdjacent(out List<IGridElement> matchingElements,element))
            {
                HandleBlast(matchingElements,element);
            }
            else
            {
                //ToDo Interactable aniation to give visual feedback
            }
        }

        private void HandleBlast(List<IGridElement> matchingElements,IGridElement pressedElement)
        {
            if (matchingElements == null || matchingElements.Count == 0)  return;
            if (matchingElements.Count >= _minimumAmountToMakeTnt)
            {
                pressedElement.UpdateElement(GridElementUpdate.UpdateToTnt);
                matchingElements.Remove(pressedElement);
            }
            RemoveElementsFromGrid(matchingElements);
            DamageObstaclesAround(matchingElements, pressedElement);
            
            AnimateElementsToScaleZero(matchingElements,() => DeactivateElements(matchingElements));
             GridSignals.Instance.onBlastCompleted.Invoke();
            GridSignals.Instance.onSetSortOrder.Invoke();
        }

        private void DamageObstaclesAround(List<IGridElement> matchingElements,IGridElement startElement)
        {
            var nearbyObstacles = FindObstaclesAroundElements(matchingElements);

            Vector2Int currentMatrixPosition;
            foreach (var obstacle in nearbyObstacles)
            {
                currentMatrixPosition = obstacle.MatrixPosition; 
                
                if(obstacle.UpdateElement(GridElementUpdate.UpdateToDamaged))
                {
                    _gridUtils.RemoveItemAt(currentMatrixPosition.x,currentMatrixPosition.y);
                }
            }
        }

        private List<IGridElement> FindObstaclesAroundElements(List<IGridElement> matchingElements)
        {
            List<IGridElement> nearObstacles = new List<IGridElement>();

            foreach (var element in matchingElements)
            {
                _gridFinder.LookForInteractablesOnAxis(out HashSet<IGridElement> foundObstacles, element,
                    x => x.Type.IsObstacle());
                nearObstacles.AddRange(foundObstacles);
            }
            return nearObstacles;
        }


        private void DeactivateElements(List<IGridElement> matchingElements)
        {
            foreach (var cube in matchingElements)
            {
                cube.ResetElement();
            }
        }

        private void RemoveElementsFromGrid(List<IGridElement> matchingElements)
        {
            foreach (var element in matchingElements)
            {
                Vector2Int pos = element.MatrixPosition;
                _gridUtils.RemoveItemAt(pos.x,pos.y);
            }
        }

        private void AnimateElementsToScaleZero(List<IGridElement> elementsToAnimate,Action onComplete)
        {
            Sequence animateSequence = DOTween.Sequence();
            foreach (var element in elementsToAnimate)
            {
               Tween anim = element.ElementTransfom.DOScale(0, 0.1f).SetEase(Ease.InQuad);
               animateSequence.Join(anim);
            }
            animateSequence.OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }
    }
}