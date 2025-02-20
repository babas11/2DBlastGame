using System.Collections.Generic;
using Script.Interfaces;

namespace Script.Keys
{
    public struct FallElementBach
    {
        public List<IGridElement> elementsToFall;
        public float offScreenValue;

        public FallElementBach(List<IGridElement> elementsToFall, float offScreenValue)
        {
            this.elementsToFall = elementsToFall;
            this.offScreenValue = offScreenValue;
        }
    }
    
}