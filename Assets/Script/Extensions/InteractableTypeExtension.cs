using System;
using Script.Enums;

namespace Script.Extensions
{
    public static class InteractableTypeExtension
    {
        public static string GetRawValue(this InteractableType type)
        {
            switch (type)
            {
                case (InteractableType.blue):
                    return "b";
                case (InteractableType.green):
                    return "g";
                case (InteractableType.red):
                    return "r";
                case (InteractableType.yellow):
                    return "y";
                case (InteractableType.vase):
                    return "v";
                case (InteractableType.box):
                    return "bo";
                case (InteractableType.stone):
                    return "s";
                default:
                    return null;
            }
            
        }
        
        public static InteractableType StringToInteractableType(this string stringValue)
        {
            switch (stringValue)
            {
                case "b":    return InteractableType.blue;
                case "g":    return InteractableType.green;
                case "r":    return InteractableType.red;
                case "y":    return InteractableType.yellow;
                case "v":    return InteractableType.vase;
                case "bo":   return InteractableType.box;
                case "s":    return InteractableType.stone;  
                case "rand": return InteractableType.random;
                default:
                    throw new ArgumentException($"This string value do not have a corresponding InteractableType : {stringValue}");
            }
        }
        public static InteractableBehaviorType StringToPoolType(this string stringValue)
        {
            switch (stringValue)
            {
                case "b":    
                case "g":    
                case "r":    
                case "y":
                case "rand":
                    return InteractableBehaviorType.Cube;
                case "v":    
                case "bo":   
                case "s":
                    return InteractableBehaviorType.Obstacle;
                default:
                    throw new ArgumentException($"This string value do not have a corresponding InteractableType : {stringValue}");
            }
        }
        
        public static InteractableType CubeTypeToInteractableType(this CubeType cubeType)
        {
            switch (cubeType)
            {
                case CubeType.Red:
                    return InteractableType.red;
                case CubeType.Green:
                    return InteractableType.green;
                case CubeType.Blue:
                    return InteractableType.blue;
                case CubeType.Yellow:
                    return InteractableType.yellow;
                default:
                    throw new ArgumentException($"This CubeType do not have a corresponding InteractableType : {cubeType}");
            }
        }
        
        public static CubeType InteractableTypeToCubeType(this InteractableType interactableType)
        {
            switch (interactableType)
            {
                case InteractableType.blue:
                    return CubeType.Blue;
                case InteractableType.green:
                    return CubeType.Green;
                case InteractableType.yellow:
                    return CubeType.Yellow;
                case InteractableType.red:
                    return CubeType.Red;
                default:
                    throw new ArgumentException($"This InteractableType is not a type of Cube : {interactableType}");
            }
        }
        
        public static bool IsCube(this InteractableType interactableType)
        {
            switch (interactableType)
            {
                case InteractableType.blue:
                case InteractableType.green:
                case InteractableType.yellow:
                case InteractableType.red:
                    return true;
                default:
                    return false;
            }
        }
        
        public static ObstaccleType InteractableTypeToObstacleType(this InteractableType interactableType)
        {
            switch (interactableType)
            {
                case InteractableType.stone:
                    return ObstaccleType.Stone;
                case InteractableType.box:
                    return ObstaccleType.Box;
                case InteractableType.vase:
                    return ObstaccleType.Vase;
                default:
                    throw new ArgumentException($"This InteractableType is not a type of Obstacle : {interactableType}");
            }
        }
        public static bool IsObstacle(this InteractableType interactableType)
        {
            switch (interactableType)
            {
                case InteractableType.stone:
                case InteractableType.box:
                case InteractableType.vase:
                    return true;
                default:
                    return false;
            }
        }
    }
}

