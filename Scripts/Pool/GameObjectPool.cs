using System.Collections.Generic;
using UnityEngine;

namespace CGame
{
    public sealed class GameObjectPool : PoolBase<GameObject>
    {
        private GameObject _template;

        private GameObjectPool(GameObject template = null, int capacity = 50) : base(capacity)
        {
            _template = template;
            this.capacity = capacity;
        }

        private static readonly Dictionary<string, GameObjectPool> _allGameObjectPool = new();
        
        public static GameObjectPool GetPool(GameObject template = null, int capacity = 50)
        {
            var key = template == null ? "" : template.name;
            if (!_allGameObjectPool.TryGetValue(key, out var value))
            {
                value = new GameObjectPool(template, capacity);
                _allGameObjectPool.Add(key, value);
                return value;
            }

            value._template = template;
            value.capacity = capacity;
            return value;
        }
        
        protected override GameObject CreateHandle() => _template != null ? Object.Instantiate(_template) : new GameObject();

        protected override void GetHandle(GameObject obj) => obj.SetActive(true);

        protected override void ReleaseHandle(GameObject obj) => obj.SetActive(false);

        protected override void ClearHandle(LinkedList<GameObject> gotLinkedList, Stack<GameObject> releasedQueue)
        {
            foreach (var gameObject in gotLinkedList)
            {
                Object.Destroy(gameObject);
            }

            while (releasedQueue.TryPop(out var gameObject))
            {
                Object.Destroy(gameObject);
            }
        }
    }
}