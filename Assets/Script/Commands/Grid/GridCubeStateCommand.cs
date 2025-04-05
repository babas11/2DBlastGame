using System.Collections.Generic;
using Script.Data.UnityObjects;
using Script.Enums;
using Script.Extensions;
using Script.Interfaces;
using Script.Managers;
using Script.Utilities.Grid;
using UnityEngine;

namespace Script.Commands.Grid
{
    public class GridCubeStateCommand
    {
        private GridFinder _gridFinder;
        private GridManipulationUtilities<IGridElement> gridUtilities => _gridFinder.GridManipulationUtilities;
        private CD_Grid _gridData;
        private Vector2Int _dimensions;

        public GridCubeStateCommand(GridFinder gridFinder, Vector2Int dimensions, CD_Grid gridData)
        {
            _gridFinder = gridFinder;
            _dimensions = dimensions;
            _gridData = gridData;
        }

        internal void Execute()
        {
            for (int y = 0; y < _dimensions.y; y++)
            {
                for (int x = 0; x < _dimensions.x; x++)
                {
                    IGridElement interactable = gridUtilities.GetItemAt(x, y);
                    if (gridUtilities.IsEmpty(x, y)) { continue;}
                    if(!interactable.Type.IsCube()) { continue;}
                    
                    List<IGridElement> currentMatches = new List<IGridElement>();
                    _gridFinder.LookInteractableForMatchingAdjacent(out currentMatches, interactable);

                    if (currentMatches.Count < _gridData.GridRuleData.minimumAmounToMakeTnt)
                    {
                        foreach (var match in currentMatches)
                        {
                            match.UpdateElement(GridElementUpdate.UpdateToDefault);
                        }
                        continue;
                    }

                    foreach (var match in currentMatches)
                    {
                        if (match.Type == InteractableType.green || match.Type == InteractableType.red ||
                            match.Type == InteractableType.blue || match.Type == InteractableType.yellow)
                        {
                            match.UpdateElement(GridElementUpdate.UpdateToCubeTnt);
                        }
                    }
                }
            }
        }
    }
}