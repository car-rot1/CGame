using System;
using System.Collections.Generic;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;
using UnityEngine.Serialization;

namespace CGame
{
    [Serializable]
    public abstract class NumericalFixedBase<T> : ISerializationCallbackReceiver
    {
        [SerializeField] private T baseValue;
        public T BaseValue
        {
            get => baseValue;
            set
            {
                if (baseValue.Equals(value))
                    return;
                baseValue = value;
                NeedRefresh();
            }
        }
#if !ODIN_INSPECTOR
        [SerializeField]
#endif
        private T finalValue;
#if ODIN_INSPECTOR
        [ShowInInspector]
#endif
        public T FinalValue
        {
            get
            {
                if (!_refresh)
                    return finalValue;
                
                _refresh = false;
                RefreshValue();
                return finalValue;
            }
        }
        public event Action OnRefreshValue; 

        private bool _refresh;

        private List<AddModifierBase<T>> _addModifiers;
        private List<AddModifierBase<T>> AddModifiers => _addModifiers ??= new List<AddModifierBase<T>>();
        private List<MultiModifierBase<T>> _multiModifiers;
        private List<MultiModifierBase<T>> MultiModifiers => _multiModifiers ??= new List<MultiModifierBase<T>>();
        private List<CoverModifierBase<T>> _coverModifiers;
        private List<CoverModifierBase<T>> CoverModifiers => _coverModifiers ??= new List<CoverModifierBase<T>>();

        protected NumericalFixedBase(T baseValue)
        {
            BaseValue = baseValue;
        }

        public void AddModifier(AddModifierBase<T> modifier)
        {
            AddModifiers.Add(modifier);
            NeedRefresh();
        }
        
        public void AddModifier(MultiModifierBase<T> modifier)
        {
            MultiModifiers.Add(modifier);
            NeedRefresh();
        }
        
        public void AddModifier(CoverModifierBase<T> modifier)
        {
            CoverModifiers.Add(modifier);
            NeedRefresh();
        }

        public void RemoveModifier(AddModifierBase<T> modifier)
        {
            AddModifiers.Remove(modifier);
            NeedRefresh();
        }
        
        public void RemoveModifier(MultiModifierBase<T> modifier)
        {
            MultiModifiers.Remove(modifier);
            NeedRefresh();
        }
        
        public void RemoveModifier(CoverModifierBase<T> modifier)
        {
            CoverModifiers.Remove(modifier);
            NeedRefresh();
        }
        
        public void NeedRefresh()
        {
            _refresh = true;
            OnRefreshValue?.Invoke();
        }

#if ODIN_INSPECTOR
        [Button]
#endif
        private void RefreshValue()
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

        public void OnBeforeSerialize()
        {
            
        }

        public void OnAfterDeserialize()
        {
            _refresh = true;
        }
    }
}