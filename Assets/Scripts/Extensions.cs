
using System;

public static class TypeEnumExtension
{

    // Extension method for InteractableType that takes an InteractableType and return its string value which is same in level Json
    public static string RawValue(this InteractableType interactableType)
    {

        switch (interactableType)
        {
            case InteractableType.red:
                return "r";
            case InteractableType.green:
                return "g";
            case InteractableType.blue:
                return "b";
            case InteractableType.yellow:
                return "y";
            case InteractableType.box:
                return "bo";
            case InteractableType.tnt:
                return "t";
            case InteractableType.stone:
                return "s";
            case InteractableType.vase:
                return "v";
            case InteractableType.random:
                return "rand";
        }

        throw new ArgumentException("Unhandled -InteractableType- enum case");

    }

    public static string NoRandomInteractableTypes(this InteractableType interactableType)
    {

        switch (interactableType)
        {
            case InteractableType.red:
                return "r";
            case InteractableType.green:
                return "g";
            case InteractableType.blue:
                return "b";
            case InteractableType.yellow:
                return "y";
            case InteractableType.box:
                return "bo";
            case InteractableType.tnt:
                return "t";
            case InteractableType.stone:
                return "s";
            case InteractableType.vase:
                return "v";
            case InteractableType.random:
                return "rand";
        }

        throw new ArgumentException("Unhandled -InteractableType- enum case");

    }


    // Extension method for string that takes a string and return related InteractableType
   public static bool TryFromRawValue(this string rawValue, out InteractableType result)
{
    switch (rawValue)
    {
        case "r":
            result = InteractableType.red;
            return true;
        case "g":
            result = InteractableType.green;
            return true;
        case "b":
            result = InteractableType.blue;
            return true;
        case "y":
            result = InteractableType.yellow;
            return true;
        case "bo":
            result = InteractableType.box;
            return true;
        case "t":
            result = InteractableType.tnt;
            return true;
        case "s":
            result = InteractableType.stone;
            return true;
        case "v":
            result = InteractableType.vase;
            return true;
        case "rand":
            result = InteractableType.random;
            return true;
        default:
            result = default; // Assign default value (if needed)
            throw new ArgumentException("Unhandled string case in -InteractableType- enum ");
    }
}




}


