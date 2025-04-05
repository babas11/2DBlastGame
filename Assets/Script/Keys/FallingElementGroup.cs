using System.Collections.Generic;
using Script.Interfaces;

namespace Script.Keys
{
    public struct FallingElementGroup
    {
        public List<IGridElement> elementsToFall;
        public float offScreenValue;

        public FallingElementGroup(List<IGridElement> elementsToFall, float offScreenValue)
        {
            this.elementsToFall = elementsToFall;
            this.offScreenValue = offScreenValue;
        }
    }
    
}