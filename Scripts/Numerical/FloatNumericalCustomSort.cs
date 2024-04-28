using System;

namespace CGame
{
    [Serializable]
    public sealed class FloatNumericalCustomSort : NumericalCustomSortBase<float>
    {
        public FloatNumericalCustomSort(float baseValue = 0) : base(baseValue)
        {
            
        }
        
        public static implicit operator FloatNumericalCustomSort(float baseValue)
        {
            return new FloatNumericalCustomSort(baseValue);
        }
    }
}