using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CGame
{
    [Obsolete("该对象池已废弃，理解起来很奇怪，请使用GameObjectPool")]
    public sealed class NewGameObjectPool : NewPoolBase<GameObject>
    {
        private readonly GameObject _template;

        public NewGameObjectPool(GameObject template = null, int capacity = 50) : base(capacity)
        {
            _template = template;
        }

        protected override GameObject CreateHandle() => _template != null ? Object.Instantiate(_template) : new GameObject();

        protected override void GetHandle(GameObject obj) => obj.SetActive(true);

        protected override void ReleaseHandle(GameObject obj) => obj.SetActive(false);

        protected override void ClearHandle(LinkedList<GameObject> linkedList)
        {
            foreach (var obj in linkedList)
            {
                Object.Destroy(obj);
            }
        }
    }
}