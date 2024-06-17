using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;
using UnityEngine.Serialization;

namespace CGame
{
    [Serializable]
    public abstract class NumericalCustomSortBase<T> : ISerializationCallbackReceiver
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
        private T _finalValue;
#if ODIN_INSPECTOR
        [ShowInInspector]
#endif
        public T FinalValue
        {
            get
            {
                if (!_refresh)
                    return _finalValue;

                _refresh = false;
                RefreshValue();
                return _finalValue;
            }
        }
        public event Action OnRefreshValue; 

        private bool _refresh;
        
        private Comparison<ModifierBase<T>> _sortComparison;

        private List<ModifierBase<T>> _modifiers;
        public List<ModifierBase<T>> Modifiers => _modifiers ??= new List<ModifierBase<T>>();

        protected NumericalCustomSortBase(T baseValue)
        {
            BaseValue = baseValue;
        }
        
        public void AddModifier(ModifierBase<T> modifier, Comparison<ModifierBase<T>> sortComparison = null)
        {
            Modifiers.Add(modifier);
            if (sortComparison != null)
                _sortComparison = sortComparison;
            NeedRefresh();
        }

        public void RemoveModifier(ModifierBase<T> modifier, Comparison<ModifierBase<T>> sortComparison = null)
        {
            Modifiers.Remove(modifier);
            if (sortComparison != null)
                _sortComparison = sortComparison;
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
            if (_sortComparison != null)
                Modifiers.Sort(_sortComparison);
            _sortComparison = null;
            _finalValue = BaseValue;
            foreach (var modifier in Modifiers)
            {
                _finalValue = modifier.ModifierValue(FinalValue);
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
