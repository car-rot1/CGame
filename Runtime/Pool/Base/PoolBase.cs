using System;
using System.Collections.Generic;
using UnityEngine;

namespace CGame
{
    public abstract class PoolBase<T> where T : class
    {
        protected int capacity;
        private int CurrentCapacity => _gotLinkedList.Count + _releasedQueue.Count;

        private readonly LinkedList<T> _gotLinkedList = new();
        private readonly Stack<T> _releasedQueue = new();

        public event Action<T> OnCreate;
        public event Action<T> OnGet;
        public event Action<T> OnRelease;
        public event Action<LinkedList<T>, Stack<T>> OnClear;

        protected PoolBase(int capacity)
        {
            this.capacity = capacity;
        }

        public void CreateReleaseObj(int num)
        {
            num = Mathf.Min(num, capacity - CurrentCapacity);
            for (var i = 0; i < num; i++)
            {
                var obj = CreateHandle();
                OnCreate?.Invoke(obj);
                _releasedQueue.Push(obj);
                ReleaseHandle(obj);
            }
        }

        public T Get()
        {
            T obj;
            if (_releasedQueue.Count <= 0)
            {
                obj = CurrentCapacity >= capacity ? null : CreateHandle();
                OnCreate?.Invoke(obj);
            }
            else
            {
                obj = _releasedQueue.Pop();
            }
            _gotLinkedList.AddFirst(obj);
            GetHandle(obj);
            OnGet?.Invoke(obj);
            
            return obj;
        }

        protected abstract T CreateHandle();
        
        protected virtual void GetHandle(T obj) { }

        public void Release(T obj)
        {
            var linkedListNode = _gotLinkedList.Find(obj);
            if (linkedListNode != null)
                _gotLinkedList.Remove(linkedListNode);
            _releasedQueue.Push(obj);
            ReleaseHandle(obj);
            OnRelease?.Invoke(obj);
        }
        
        protected virtual void ReleaseHandle(T obj) { }
        
        public void Clear()
        {
            ClearHandle(_gotLinkedList, _releasedQueue);
            OnClear?.Invoke(_gotLinkedList, _releasedQueue);
            _gotLinkedList.Clear();
            _releasedQueue.Clear();
        }

        protected virtual void ClearHandle(LinkedList<T> gotLinkedList, Stack<T> releasedQueue) { }
    }
}