using System;
using System.Collections.Generic;
using UnityEngine;

namespace CGame
{
    /*
     * 该对象池的理解为：对象Get后，就不归对象池管理了，但是使用中的物体还是会影响容量。
     * 清理时，已使用的物体将不再计入容量中，并且不会被清除；
     */
    [Obsolete("该对象池已废弃，理解起来很奇怪，请使用PoolBase")]
    public abstract class NewPoolBase<T> where T : class
    {
        private readonly int _capacity;
        private int _getCapacity;
        private int CurrentCapacity => _linkedList.Count + _getCapacity;

        private readonly LinkedList<T> _linkedList;
        
        public event Action<T> OnCreate; 
        public event Action<T> OnGet;
        public event Action<T> OnRelease; 
        public event Action<LinkedList<T>> OnClear; 

        protected NewPoolBase(int capacity)
        {
            _capacity = capacity;
            _linkedList = new LinkedList<T>();
        }

        public T Get()
        {
            T obj;
            if (_linkedList.Count <= 0)
            {
                obj = CurrentCapacity >= _capacity ? null : CreateHandle();
                OnCreate?.Invoke(obj);
            }
            else
            {
                obj = _linkedList.First.Value;
                _linkedList.RemoveFirst();
            }

            _getCapacity++;
            GetHandle(obj);
            OnGet?.Invoke(obj);
            
            return obj;
        }

        protected abstract T CreateHandle();
        
        protected virtual void GetHandle(T obj) { }

        public void Release(T obj)
        {
            if (_linkedList.Count > 0 && ReferenceEquals(_linkedList.First.Value, obj))
                Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
            _linkedList.AddFirst(obj);
            if (_getCapacity > 0)
                _getCapacity--;
            ReleaseHandle(obj);
            OnRelease?.Invoke(obj);
        }
        
        protected virtual void ReleaseHandle(T obj) { }

        // 清理对象池后，已激活的物体将不计入容量中
        public void Clear()
        {
            _getCapacity = 0;
            ClearHandle(_linkedList);
            OnClear?.Invoke(_linkedList);
            _linkedList.Clear();
        }

        protected virtual void ClearHandle(LinkedList<T> linkedList) { }
    }
}