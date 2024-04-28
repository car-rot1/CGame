using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CGame
{
    [Obsolete("该对象池已废弃，理解起来很奇怪，请使用ComponentPool")]
    public sealed class NewComponentPool<T> : NewPoolBase<T> where T : Component
    {
        private readonly T _template;

        public NewComponentPool(T template = null, int capacity = 50) : base(capacity)
        {
            _template = template;
        }

        protected override T CreateHandle() => _template != null ? Object.Instantiate(_template) : new GameObject().AddComponent<T>();

        protected override void GetHandle(T obj) => obj.gameObject.SetActive(true);

        protected override void ReleaseHandle(T obj) => obj.gameObject.SetActive(false);

        protected override void ClearHandle(LinkedList<T> linkedList)
        {
            foreach (var obj in linkedList)
            {
                Object.Destroy(obj);
            }
        }
    }
}