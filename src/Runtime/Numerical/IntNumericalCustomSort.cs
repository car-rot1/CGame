using System;

namespace CGame
{
    [Serializable]
    public sealed class IntNumericalCustomSort : NumericalCustomSortBase<int>
    {
        public IntNumericalCustomSort(int baseValue = 0) : base(baseValue)
        {
            
        }
        
        public static implicit operator IntNumericalCustomSort(int baseValue)
        {
            return new IntNumericalCustomSort(baseValue);
        }
    }
}