using System;
using System.Collections.Generic;
using System.Linq;
using Script.Interfaces;
using UnityEngine;

namespace Script.Utilities.Grid
{
    public class  GridFinder
    {
        private GridManipulationUtilities<IGridElement> _gridManipulationUtilities;
        public GridManipulationUtilities<IGridElement> GridManipulationUtilities => _gridManipulationUtilities;
        public GridFinder(GridManipulationUtilities<IGridElement> gridManipulationUtilities)
        {
            this._gridManipulationUtilities = gridManipulationUtilities;
        }
        
        public bool LookInteractableForMatchingAdjacent(out List<IGridElement> matchList,
            IGridElement startInteractable = null)
        {
            List<IGridElement> matches = new List<IGridElement>();

            if (startInteractable != null)
            {
                SearchForMatches(startInteractable, matches);
            }

            matchList = matches;

            return matches.Count > 1;
        }

        public void SearchForMatches(IGridElement startInteractable, List<IGridElement> matches)
        {
            if (startInteractable == null) { Debug.LogError("Start interactable is null."); }

            if (matches == null) { Debug.LogError("Matches list is null."); }

            // Add the starting interactable to the matches list
            matches.Add(startInteractable);

            // Define the four primary directions to search
            Vector2Int[] directions = new Vector2Int[]
            {
                Vector2Int.left,
                Vector2Int.right,
                Vector2Int.up,
                Vector2Int.down
            };

            foreach (var direction in directions)
            {
                // Calculate the new position based on the current direction
                Vector2Int newPos = startInteractable.MatrixPosition + direction;

                // Check if the new position is within grid bounds
                if (!_gridManipulationUtilities.CheckBounds(newPos)) { continue; }

                // Check if the new position is not empty
                if (_gridManipulationUtilities.IsEmpty(newPos.x, newPos.y)) { continue; }

                // Get the interactable at the new position
                IGridElement adjacentInteractable = _gridManipulationUtilities.GetItemAt(newPos.x, newPos.y);

                // Check if the adjacent interactable matches the type T
                if (adjacentInteractable != null &&
                    adjacentInteractable.Type == startInteractable.Type)
                {
                    // Check if it's not already in the matches list
                    if (!matches.Contains(adjacentInteractable))
                    {
                        // Recursively search from the adjacent interactable
                        SearchForMatches(adjacentInteractable, matches);
                    }
                }
            }
        }

        public List<IGridElement> GetInteractablesWithinRange(IGridElement centerInteractable, int range,
            InteractableType type = InteractableType.random, bool involveCenter = false)
        {
            Vector2Int blastPosition = centerInteractable.MatrixPosition;
            List<IGridElement> interactablesInRange = new List<IGridElement>();

            for (int x = blastPosition.x - range; x <= blastPosition.x + range; x++)
            {
                for (int y = blastPosition.y - range; y <= blastPosition.y + range; y++)
                {
                    Vector2Int position = new Vector2Int(x, y);
                    if (_gridManipulationUtilities.CheckBounds(position) && !_gridManipulationUtilities.IsEmpty(position.x, position.y))
                    {
                        IGridElement interactable = _gridManipulationUtilities.GetItemAt(position.x, position.y);
                        
                        if (interactable != null)
                        {
                            if (interactable.Type == type || type == InteractableType.random)
                            {
                                interactablesInRange.Add(interactable);
                            }
                        }
                    }
                }
            }

            if (!involveCenter)
                interactablesInRange.Remove(centerInteractable);
            return interactablesInRange;
        }
        public List<IGridElement> GetInteractablesWithinRange(
            IGridElement centerInteractable, 
            int range,
            Func<IGridElement, bool> condition,
            bool involveCenter = false)
        {
            Vector2Int blastPosition = centerInteractable.MatrixPosition;
            List<IGridElement> interactablesInRange = new List<IGridElement>();

            for (int x = blastPosition.x - range; x <= blastPosition.x + range; x++)
            {
                for (int y = blastPosition.y - range; y <= blastPosition.y + range; y++)
                {
                    Vector2Int position = new Vector2Int(x, y);
                    if (_gridManipulationUtilities.CheckBounds(position) && !_gridManipulationUtilities.IsEmpty(position.x, position.y))
                    {
                        IGridElement interactable = _gridManipulationUtilities.GetItemAt(position.x, position.y);
                        
                        if (interactable != null)
                        {
                            if (condition(interactable))
                            {
                                interactablesInRange.Add(interactable);
                            }
                        }
                    }
                }
            }

            if (!involveCenter)
                interactablesInRange.Remove(centerInteractable);
            return interactablesInRange;
        }

        public List<IGridElement> GetAllCubesWithinRange(IGridElement centerInteractable, int range,
            bool involveCenter = false)

        {
            Vector2Int blastPosition =
                new Vector2Int(centerInteractable.MatrixPosition.x, centerInteractable.MatrixPosition.y);
            List<IGridElement> cubesInRange = new List<IGridElement>();

            for (int x = blastPosition.x - range; x <= blastPosition.x + range; x++)
            {
                for (int y = blastPosition.y - range; y <= blastPosition.y + range; y++)
                {
                    Vector2Int position = new Vector2Int(x, y);
                    if (_gridManipulationUtilities.CheckBounds(position) && !_gridManipulationUtilities.IsEmpty(position.x, position.y))
                    {
                        IGridElement interactable = _gridManipulationUtilities.GetItemAt(position.x, position.y);

                        if (interactable != null)
                        {
                            if (interactable.Type == InteractableType.blue ||
                                interactable.Type == InteractableType.green ||
                                interactable.Type == InteractableType.red ||
                                interactable.Type == InteractableType.yellow)
                                cubesInRange.Add(interactable);
                        }
                    }
                }
            }

            if (!involveCenter)
                cubesInRange.Remove(centerInteractable);
            return cubesInRange;
        }


        public bool LookForInteractablesOnAxis(out HashSet<IGridElement> nearInteractables,IGridElement startInteractable,Func<IGridElement, bool> condition)
        {
            nearInteractables = new HashSet<IGridElement>();
            Vector2Int[] directions = { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down };

            foreach (var direction in directions)
            {
                Vector2Int pos = startInteractable.MatrixPosition + direction;
                
                if (_gridManipulationUtilities.CheckBounds(pos) && !_gridManipulationUtilities.IsEmpty(pos.x, pos.y))
                {
                    IGridElement item = _gridManipulationUtilities.GetItemAt(pos.x, pos.y);
                    if (condition(item))
                    {
                        nearInteractables.Add(item);
                    }
                }
            }

            return nearInteractables.Count > 0;
        }
        
        public List<IGridElement> GetChainElementsInRange(IGridElement initialTnt,InteractableType aimType,int defaultRange,bool isMegaTnt = false,int megaRange = 3)
        {
            Queue<IGridElement> queue = new Queue<IGridElement>();
            HashSet<IGridElement> visited = new HashSet<IGridElement>();

            queue.Enqueue(initialTnt);
            visited.Add(initialTnt);
            bool isMega = isMegaTnt;

            while (queue.Count > 0)
            {
                IGridElement current = queue.Dequeue();
            
                int range = isMegaTnt? megaRange : defaultRange;
                if (isMega) {isMega = false;}
            
                var nearTnts = GetInteractablesWithinRange(current, range,aimType);
                foreach (var tnt in nearTnts)
                {
                    if (!visited.Contains(tnt))
                    {
                        visited.Add(tnt);
                        queue.Enqueue(tnt);
                    }
                }
            }
            return visited.ToList();
        }

    }
}