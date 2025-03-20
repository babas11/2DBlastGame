using System.Collections.Generic;
using System.Linq;
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
    public class TntExplodeStrategy : IExplodeStrategy
    {
        private GridManipulationUtilities<IGridElement> _gridUtils;
        private GridFinder _gridFinder;
        private const int NormalRange = 2;
        private const int SynergyRange = 3;

        public TntExplodeStrategy(GridManipulationUtilities<IGridElement> gridUtils, GridFinder gridFinder)
        {
            _gridUtils = gridUtils;
            _gridFinder = gridFinder;
        }

        public void Explode(IGridElement startTnt)
        {
            startTnt.BringElementFront();
            bool isMegaTnt =_gridFinder.LookForInteractablesOnAxis(out HashSet<IGridElement> nearTnts, startTnt, 
                x => x.Type == InteractableType.tnt);
            HashSet<IGridElement> visited = new HashSet<IGridElement>();

            if (isMegaTnt)
            {
                CombineThenExplodeFistMegaTnt(startTnt, nearTnts.ToList(), visited);
            }
            else
            {
                List<IGridElement> firstWave = new List<IGridElement> { startTnt };
                ExplodeWave(firstWave, visited);
            }
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


        private void CombineThenExplodeFistMegaTnt(IGridElement mainTnt,List<IGridElement> otherTnts, HashSet<IGridElement> visited)
        {
            visited.Add(mainTnt);
            
            Sequence seq = DOTween.Sequence();
            // 1) Move the other TNT to the mainTnt's position (combine)
            foreach (var tnt in otherTnts)
            {
                seq.Join(
                    tnt.ElementTransfom.DOMove(
                        mainTnt.ElementTransfom.position, 0.2f
                    )
                        .OnComplete(() =>
                        {
                            RemoveTnt(tnt);
                        })
                );
            }

            seq.Append(
                
                mainTnt.ElementTransfom.DOScale(2.0f, 0.15f * 0.7f).SetEase(Ease.OutElastic)
            );
            seq.Join(
                mainTnt.ElementTransfom.DORotate(new Vector3(0, 0, 360), 0.15f * 0.7f, RotateMode.FastBeyond360)
                    .SetEase(Ease.OutElastic)
            );
            seq.Join(
                mainTnt.ElementTransfom.DOScale(0f, 0.15f * 0.7f).SetEase(Ease.InElastic)
            );
            

            // 4) Then do the "mega" explosion with synergy range
            seq.AppendCallback(() =>
            {
                DamageSurroundingElements(mainTnt, SynergyRange);
                
            });
            
            // 5) After finishing this wave, gather the next wave
            List<IGridElement> nextWave = new List<IGridElement>();
            
            seq.OnComplete(() =>
            {
                List<IGridElement> newlyFoundTnt = HandleTntExplosion(mainTnt,SynergyRange);

                foreach (var newTnt in newlyFoundTnt)
                {
                    if (!visited.Contains(newTnt))
                    {
                        nextWave.Add(newTnt);
                    }
                }
                ExplodeWave(nextWave, visited); 
            });

            seq.Play();
        }
        private List<IGridElement> HandleTntExplosion(IGridElement tnt, int range = NormalRange)
        {
            List<IGridElement> chainTnt = _gridFinder.GetChainElementsInRange(tnt, InteractableType.tnt, range);
            chainTnt.Remove(tnt);
            DamageSurroundingElements(tnt, range);
            RemoveTnt(tnt);
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
            InteractableType currentType;
            CleanedObstacles cleanedObstacles = new(); 
            Dictionary<ObstaccleType, byte> Obstacles = new();
            foreach (var elem in nearElements)
            {
                currentPos = elem.MatrixPosition;
                currentType = elem.Type;
                
                if (elem.UpdateElement(GridElementUpdate.UpdateToDamaged))
                {
                    _gridUtils.RemoveItemAt(currentPos.x,currentPos.y);
                    
                    if (currentType.IsObstacle())
                    {
                        ObstaccleType obsType = currentType.InteractableTypeToObstacleType();
                        if (!Obstacles.ContainsKey(obsType))
                        {
                            Obstacles.Add(obsType, 1);
                        }
                        else
                        {
                            Obstacles[obsType]++;
                        }
                    }
                }
            }
            cleanedObstacles.Obstacles = Obstacles;
            LevelObjectivesSignals.Instance.onObjectiveCleaned(cleanedObstacles);
        }

        private void RemoveTnt(IGridElement tnt)
        {
            Vector2Int pos = tnt.MatrixPosition;
            _gridUtils.RemoveItemAt(pos.x, pos.y);
            tnt.UpdateElement(GridElementUpdate.UpdateToTntExplode);
        }

        private void OnAllExplosionsComplete()
        {
            GridSignals.Instance.onBlastCompleted?.Invoke();
        }
    }
}