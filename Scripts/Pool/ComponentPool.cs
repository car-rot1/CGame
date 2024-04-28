using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CGame
{
    [Obsolete("已废弃，该类操控的对象还是对应的GameObject，请使用GameObjectPool")]
    public sealed class ComponentPool<T> : PoolBase<T> where T : Component
    {
        private T _template;

        private ComponentPool(T template = null, int capacity = 50) : base(capacity)
        {
            _template = template;
            this.capacity = capacity;
        }
        
        private static ComponentPool<T> _componentPool;
        
        public static ComponentPool<T> GetPool(T template = null, int capacity = 50)
        {
            if (_componentPool == null)
            {
                _componentPool = new ComponentPool<T>(template, capacity);
            }
            else
            {
                _componentPool._template = template;
                _componentPool.capacity = capacity;
            }
            return _componentPool;
        }

        protected override T CreateHandle() => _template != null ? Object.Instantiate(_template) : new GameObject().AddComponent<T>();
        
        protected override void GetHandle(T obj) => obj.gameObject.SetActive(true);
        
        protected override void ReleaseHandle(T obj) => obj.gameObject.SetActive(false);
        
        protected override void ClearHandle(LinkedList<T> gotLinkedList, Stack<T> releasedQueue)
        {
            foreach (var component in gotLinkedList)
            {
                Object.Destroy(component);
            }

            while (releasedQueue.TryPop(out var component))
            {
                Object.Destroy(component);
            }
        }
    }
}