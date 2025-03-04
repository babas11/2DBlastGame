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
    public class TntExplodeStrategy : IExplodeStrategy
    {
        private GridManipulationUtilities<IGridElement> _gridUtils;
        private GridFinder _gridFinder;

        public TntExplodeStrategy(GridManipulationUtilities<IGridElement> gridUtils, GridFinder gridFinder)
        {
            _gridUtils = gridUtils;
            _gridFinder = gridFinder;
        }

        public void Explode(IGridElement startTnt)
        {
            HashSet<IGridElement> visited = new HashSet<IGridElement>();
        
            List<IGridElement> firstWave = new List<IGridElement> { startTnt };

            ExplodeWave(firstWave, visited);
        }

        private void ExplodeWave(List<IGridElement> currentWave, HashSet<IGridElement> visited)
        {
            currentWave.RemoveAll(tnt => visited.Contains(tnt));
            
            if (currentWave.Count == 0)
            {
                OnAllExplosionsComplete();
                return;
            }
            
            foreach (var tnt in currentWave)
            {
                visited.Add(tnt);
            }

            List<IGridElement> nextWave = new List<IGridElement>();

            Sequence waveSequence = DOTween.Sequence();

            foreach (var tnt in currentWave)
            {
                Tween explodeAnim = tnt.ElementTransfom
                    .DOScale(1.5f, 0.3f)
                    .SetEase(Ease.InBounce);
                
                waveSequence.Join(explodeAnim);
                
                waveSequence.AppendCallback(() =>
                {
                    List<IGridElement> newlyFoundTnt = HandleTntExplosion(tnt);

                    foreach (var newTnt in newlyFoundTnt)
                    {
                        if (!visited.Contains(newTnt))
                        {
                            nextWave.Add(newTnt);
                        }
                    }
                });
            }
        
            waveSequence.OnComplete(() =>
            {
                ExplodeWave(nextWave, visited);
            });

            waveSequence.Play();
        }

        private List<IGridElement> HandleTntExplosion(IGridElement tnt)
        {
            List<IGridElement> chainTnt = _gridFinder.GetChainElementsInRange(tnt, InteractableType.tnt, 2);
            chainTnt.Remove(tnt);
            
            DamageSurroundingElements(tnt, 2);
            RemoveTntItself(tnt);
            
            return chainTnt;
        }

        private void DamageSurroundingElements(IGridElement tnt, int range)
        {
            List<IGridElement> nearElements = _gridFinder.GetInteractablesWithinRange(
                tnt,
                range,
                x => x.Type.IsCube() || x.Type.IsObstacle()
            );

            Vector2Int currentPos;
            foreach (var elem in nearElements)
            {
                currentPos = elem.MatrixPosition;
                if (elem.UpdateElement(GridElementUpdate.UpdateToDamaged))
                {
                    _gridUtils.RemoveItemAt(currentPos.x,currentPos.y);
                }
            }
        }

        private void RemoveTntItself(IGridElement tnt)
        {
            Vector2Int pos = tnt.MatrixPosition;
            _gridUtils.RemoveItemAt(pos.x, pos.y);
            tnt.UpdateElement(GridElementUpdate.UpdateToDamaged);
        }

        private void OnAllExplosionsComplete()
        {
            GridSignals.Instance.onBlastCompleted?.Invoke();
            GridSignals.Instance.onSetSortOrder?.Invoke();
        }
    }
}