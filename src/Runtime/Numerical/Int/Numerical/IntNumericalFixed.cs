using System;
using System.Collections.Generic;

namespace CGame
{
    [Serializable]
    public sealed class IntNumericalFixed : NumericalBase<int>
    {
        public static implicit operator IntNumericalFixed(int baseValue)
        {
            return new IntNumericalFixed(baseValue);
        }
        
        private List<IntAddModifier> _addModifiers;
        private List<IntAddModifier> AddModifiers => _addModifiers ??= new List<IntAddModifier>();
        private List<IntMultiModifier> _multiModifiers;
        private List<IntMultiModifier> MultiModifiers => _multiModifiers ??= new List<IntMultiModifier>();
        private List<IntCoverModifier> _coverModifiers;
        private List<IntCoverModifier> CoverModifiers => _coverModifiers ??= new List<IntCoverModifier>();
        
        public IntNumericalFixed(int baseValue = 0) : base(baseValue) { }
        
        public void AddModifier(IntAddModifier modifier)
        {
            AddModifiers.Add(modifier);
            NeedRefresh();
        }
        
        public void AddModifier(IntMultiModifier modifier)
        {
            MultiModifiers.Add(modifier);
            NeedRefresh();
        }
        
        public void AddModifier(IntCoverModifier modifier)
        {
            CoverModifiers.Add(modifier);
            NeedRefresh();
        }

        public void RemoveModifier(IntAddModifier modifier)
        {
            AddModifiers.Remove(modifier);
            NeedRefresh();
        }
        
        public void RemoveModifier(IntMultiModifier modifier)
        {
            MultiModifiers.Remove(modifier);
            NeedRefresh();
        }
        
        public void RemoveModifier(IntCoverModifier modifier)
        {
            CoverModifiers.Remove(modifier);
            NeedRefresh();
        }
        
        protected override void RefreshValue()
        {
            //若存在覆盖修饰器可以直接取最后一个赋值，而不需要遍历其他的修饰器
            if (CoverModifiers.Count > 0)
            {
                finalValue = CoverModifiers[^1].ModifierValue(finalValue);
                return;
            }
            
            finalValue = BaseValue;
            foreach (var addModifier in AddModifiers)
            {
                finalValue = addModifier.ModifierValue(finalValue);
            }
            foreach (var multiModifier in MultiModifiers)
            {
                finalValue = multiModifier.ModifierValue(finalValue);
            }
        }
    }
}