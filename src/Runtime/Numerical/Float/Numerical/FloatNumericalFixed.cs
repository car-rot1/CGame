using System;
using System.Collections.Generic;

namespace CGame
{
    [Serializable]
    public sealed class FloatNumericalFixed : NumericalBase<float>
    {
        public static implicit operator FloatNumericalFixed(float baseValue)
        {
            return new FloatNumericalFixed(baseValue);
        }
        
        private List<FloatAddModifier> _addModifiers;
        private List<FloatAddModifier> AddModifiers => _addModifiers ??= new List<FloatAddModifier>();
        private List<FloatMultiModifier> _multiModifiers;
        private List<FloatMultiModifier> MultiModifiers => _multiModifiers ??= new List<FloatMultiModifier>();
        private List<FloatCoverModifier> _coverModifiers;
        private List<FloatCoverModifier> CoverModifiers => _coverModifiers ??= new List<FloatCoverModifier>();
        
        public FloatNumericalFixed(float baseValue = 0) : base(baseValue) { }
        
        public void AddModifier(FloatAddModifier modifier)
        {
            AddModifiers.Add(modifier);
            NeedRefresh();
        }
        
        public void AddModifier(FloatMultiModifier modifier)
        {
            MultiModifiers.Add(modifier);
            NeedRefresh();
        }
        
        public void AddModifier(FloatCoverModifier modifier)
        {
            CoverModifiers.Add(modifier);
            NeedRefresh();
        }

        public void RemoveModifier(FloatAddModifier modifier)
        {
            AddModifiers.Remove(modifier);
            NeedRefresh();
        }
        
        public void RemoveModifier(FloatMultiModifier modifier)
        {
            MultiModifiers.Remove(modifier);
            NeedRefresh();
        }
        
        public void RemoveModifier(FloatCoverModifier modifier)
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