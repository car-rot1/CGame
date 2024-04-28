using System;

namespace CGame
{
    [Serializable]
    public sealed class IntNumericalFixed : NumericalFixedBase<int>
    {
        public IntNumericalFixed(int baseValue = 0) : base(baseValue)
        {
            
        }
        
        public static implicit operator IntNumericalFixed(int baseValue)
        {
            return new IntNumericalFixed(baseValue);
        }
    }
}