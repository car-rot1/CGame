using System;
using System.Collections;
using System.Collections.Generic;

namespace CGame
{
    public abstract class InventoryBase<T> : IEnumerable<KeyValuePair<string, (T item, int num)>> where T : IInventoryItem
    {
        public event Action<T, int> OnAddItem; 
        public event Action<T, int> OnRemoveItem; 
        
        private readonly Dictionary<string, (T item, int num)> _items = new();

        public virtual void AddItem(T item, int num = 1)
        {
            var nowNum = 0;
            var key = item.Name;
            if (_items.TryGetValue(key, out var itemInfo))
                nowNum = itemInfo.num;

            nowNum += num;
            _items[key] = (item, nowNum);
            OnAddItem?.Invoke(item, num);
        }

        public virtual void RemoveItem(T item, int num = 1)
        {
            var nowNum = 0;
            var key = item.Name;
            if (_items.TryGetValue(key, out var itemInfo))
                nowNum = itemInfo.num;
            
            nowNum -= num;
            if (nowNum <= 0)
                _items.Remove(key);
            else
                _items[key] = (item, nowNum);
            OnRemoveItem?.Invoke(item, num);
        }

        public IEnumerable<(T item, int num)> AllItem => _items.Values;

        public IEnumerator<KeyValuePair<string, (T item, int num)>> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => _items.Count;
    }
}