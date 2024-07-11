using System;
using System.Collections.Generic;

namespace CGame
{
    [Serializable]
    public class IntNumericalCustomSort : NumericalBase<int>
    {
        public static implicit operator IntNumericalCustomSort(int baseValue)
        {
            return new IntNumericalCustomSort(baseValue);
        }
        
        private Comparison<IntModifierBase> _sortComparison;

        private List<IntModifierBase> _modifiers;
        public List<IntModifierBase> Modifiers => _modifiers ??= new List<IntModifierBase>();
        
        public IntNumericalCustomSort(int baseValue) : base(baseValue) { }
        
        public void AddModifier(IntModifierBase modifier, Comparison<IntModifierBase> sortComparison = null)
        {
            Modifiers.Add(modifier);
            if (sortComparison != null)
                _sortComparison = sortComparison;
            NeedRefresh();
        }

        public void RemoveModifier(IntModifierBase modifier, Comparison<IntModifierBase> sortComparison = null)
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