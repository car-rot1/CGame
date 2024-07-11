using System;
using System.Collections.Generic;

namespace CGame
{
    [Serializable]
    public class FloatNumericalCustomSort : NumericalBase<float>
    {
        public static implicit operator FloatNumericalCustomSort(float baseValue)
        {
            return new FloatNumericalCustomSort(baseValue);
        }
        
        private Comparison<FloatModifierBase> _sortComparison;

        private List<FloatModifierBase> _modifiers;
        public List<FloatModifierBase> Modifiers => _modifiers ??= new List<FloatModifierBase>();
        
        public FloatNumericalCustomSort(float baseValue) : base(baseValue) { }
        
        public void AddModifier(FloatModifierBase modifier, Comparison<FloatModifierBase> sortComparison = null)
        {
            Modifiers.Add(modifier);
            if (sortComparison != null)
                _sortComparison = sortComparison;
            NeedRefresh();
        }

        public void RemoveModifier(FloatModifierBase modifier, Comparison<FloatModifierBase> sortComparison = null)
        {
            Modifiers.Remove(modifier);
            if (sortComparison != null)
                _sortComparison = sortComparison;
            NeedRefresh();
        }

        protected override void RefreshValue()
        {
            if (_sortComparison != null)
                Modifiers.Sort(_sortComparison);
            _sortComparison = null;
            finalValue = BaseValue;
            foreach (var modifier in Modifiers)
            {
                finalValue = modifier.ModifierValue(FinalValue);
            }
        }
    }
}