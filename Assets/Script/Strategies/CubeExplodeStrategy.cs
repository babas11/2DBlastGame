using System;
using System.Collections.Generic;
using DG.Tweening;
using Script.Enums;
using Script.Extensions;
using Script.Interfaces;
using Script.Keys;
using Script.Signals;
using Script.Utilities.Grid;
using UnityEngine;

namespace Script.Strategies
{
    public class CubeExplodeStrategy : IExplodeStrategy
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
            if (_gridFinder.LookInteractableForMatchingAdjacent(out List<IGridElement> matchingElements, element))
            {
                HandleBlast(matchingElements, element);
            }
            else
            {
                //ToDo Interactable animation to give visual feedback
            }
        }

        private void HandleBlast(List<IGridElement> matchingElements, IGridElement pressedElement)
        {
            if (matchingElements == null || matchingElements.Count == 0) return;
            bool isTnt = false;
            if (matchingElements.Count >= _minimumAmountToMakeTnt)
            {
                pressedElement.UpdateElement(GridElementUpdate.UpdateToTnt);
                isTnt = true;
            }

            DamageObstaclesAround(matchingElements, pressedElement);
            if (isTnt) matchingElements.Remove(pressedElement);
            RemoveElementsFromGrid(matchingElements);
            AnimateElementsToScaleZero(matchingElements, () => DeactivateElements(matchingElements));

            GridSignals.Instance.onBlastCompleted.Invoke();
        }

        private void DamageObstaclesAround(List<IGridElement> matchingElements, IGridElement startElement)
        {
            var nearbyObstacles = FindObstaclesAroundElements(matchingElements);
            if (nearbyObstacles.Count == 0) return;
            ObstaccleType currentType;
            CleanedObstacles cleanedObstacles = new();
            Dictionary<ObstaccleType, byte> Obstacles = new();
            Vector2Int currentMatrixPosition;
            foreach (var obstacle in nearbyObstacles)
            {
                currentMatrixPosition = obstacle.MatrixPosition;
                currentType = obstacle.Type.InteractableTypeToObstacleType();

                if (obstacle.UpdateElement(GridElementUpdate.UpdateToDamaged))
                {
                    _gridUtils.RemoveItemAt(currentMatrixPosition.x, currentMatrixPosition.y);
                    if (!Obstacles.ContainsKey(currentType))
                    {
                        Obstacles.Add(currentType, 1);
                    }
                    else
                    {
                        Obstacles[currentType]++;
                    }
                }
            }

            cleanedObstacles.Obstacles = Obstacles;
            LevelObjectivesSignals.Instance.onObjectiveCleaned(cleanedObstacles);
        }

        private HashSet<IGridElement> FindObstaclesAroundElements(List<IGridElement> matchingElements)
        {
            HashSet<IGridElement> nearObstacles = new HashSet<IGridElement>();

            foreach (var element in matchingElements)
            {
                _gridFinder.LookForInteractablesOnAxis(out HashSet<IGridElement> foundObstacles, element,
                    x => x.Type.IsObstacle() && !x.OnlyPowerDamage);
                foreach (var obstacle in foundObstacles)
                {
                    nearObstacles.Add(obstacle);
                }
            }

            return nearObstacles;
        }


        private void DeactivateElements(List<IGridElement> matchingElements)
        {
            foreach (var cube in matchingElements)
            {
                cube.UpdateElement(GridElementUpdate.UpdateToDamaged);
            }
        }

        private void RemoveElementsFromGrid(List<IGridElement> matchingElements)
        {
            foreach (var element in matchingElements)
            {
                Vector2Int pos = element.MatrixPosition;
                _gridUtils.RemoveItemAt(pos.x, pos.y);
            }
        }

        private void AnimateElementsToScaleZero(List<IGridElement> elementsToAnimate, Action onComplete)
        {
            Sequence animateSequence = DOTween.Sequence();
            foreach (var element in elementsToAnimate)
            {
                Tween anim = element.ElementTransfom.DOScale(0, 0.1f).SetEase(Ease.InQuad);
                animateSequence.Join(anim);
            }

            animateSequence.OnComplete(() => { onComplete?.Invoke(); });
        }

        private void AnimateElementZeroToOne(IGridElement elementToAnimate, Action onComplete = null)
        {
            Tween anim = elementToAnimate.ElementTransfom.DOScale(1, 0.1f).SetEase(Ease.InQuad).From(Vector3.zero);
            anim.OnComplete(() => { onComplete?.Invoke(); });
        }
    }
}