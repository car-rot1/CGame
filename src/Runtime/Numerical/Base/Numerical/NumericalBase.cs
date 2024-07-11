using System;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace CGame
{
    [Serializable]
    public abstract class NumericalBase<T> : ISerializationCallbackReceiver
    {
        [SerializeField] protected T baseValue;
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
        protected T finalValue;
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

        protected NumericalBase(T baseValue)
        {
            BaseValue = baseValue;
        }

        public void NeedRefresh()
        {
            _refresh = true;
            OnRefreshValue?.Invoke();
            OnNeedRefresh();
        }
        
        protected virtual void OnNeedRefresh() { }

#if ODIN_INSPECTOR
        [Button]
#endif
        protected abstract void RefreshValue();

        public virtual void OnBeforeSerialize()
        {
            
        }

        public virtual void OnAfterDeserialize()
        {
            _refresh = true;
        }
    }
}
