using System;

namespace CGame
{
    [Serializable]
    public sealed class FloatNumericalFixed : NumericalFixedBase<float>
    {
        public FloatNumericalFixed(float baseValue = 0) : base(baseValue)
        {
            
        }
        
        public static implicit operator FloatNumericalFixed(float baseValue)
        {
            return new FloatNumericalFixed(baseValue);
        }
    }
}