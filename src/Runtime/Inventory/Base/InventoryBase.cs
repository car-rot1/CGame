using System;
using System.Collections.Generic;
using UnityEngine;

namespace CGame
{
    public struct InventorySlotInfo<T> where T : IInventoryItem
    {
        public T item;
        public int num;

        public InventorySlotInfo(T item, int num)
        {
            this.item = item;
            this.num = num;
        }
    }
    
    public abstract class InventoryBase<T> where T : IInventoryItem
    {
        protected InventorySlotInfo<T>[] inventorySlotInfos;
        private int _freeCapacity;
        public event Action<T, int, int> OnAddItem; 
        public event Action<T, int, int> OnRemoveItem; 

        private readonly List<int> _freeInventorySlotTemp;

        protected InventoryBase(int capacity)
        {
            if (capacity <= 0)
                throw new Exception("Capacity can not <= 0");

            inventorySlotInfos = new InventorySlotInfo<T>[capacity];
            for (var i = 0; i < inventorySlotInfos.Length; i++)
                inventorySlotInfos[i] = new InventorySlotInfo<T>(default, 0);

            _freeCapacity = capacity;
            _freeInventorySlotTemp = new List<int>(capacity);
        }

        public int AddItem(T item, int num)
        {
            if (_freeCapacity == inventorySlotInfos.Length)
            {
                var index = 0;
                while (_freeCapacity > 0)
                {
                    var slotInfo = inventorySlotInfos[index];
                    var addNum = Mathf.Min(num, item.MaxNum);
                    slotInfo.item = item;
                    slotInfo.num = addNum;
                    inventorySlotInfos[index] = slotInfo;
                    OnAddItem?.Invoke(item, index, addNum);
                    index++;
                    _freeCapacity--;
                    num -= item.MaxNum;
                    if (num <= 0)
                        return 0;
                }
                return num;
            }
            
            _freeInventorySlotTemp.Clear();
            for (var i = 0; i < inventorySlotInfos.Length; i++)
            {
                var slotInfo = inventorySlotInfos[i];
                if (EqualityComparer<T>.Default.Equals(slotInfo.item, default))
                {
                    _freeInventorySlotTemp.Add(i);
                    continue;
                }
                
                if (slotInfo.item.Name == item.Name && slotInfo.num < item.MaxNum)
                {
                    var canAddNum = item.MaxNum - slotInfo.num;
                    var addNum = Mathf.Min(num, canAddNum);
                    slotInfo.num += addNum;
                    inventorySlotInfos[i] = slotInfo;
                    OnAddItem?.Invoke(item, i, addNum);
                    num -= canAddNum;
                    if (num <= 0)
                        return 0;
                }
            }
            
            foreach (var i in _freeInventorySlotTemp)
            {
                var slotInfo = inventorySlotInfos[i];
                var addNum = Mathf.Min(num, item.MaxNum);
                slotInfo.item = item;
                slotInfo.num = addNum;
                inventorySlotInfos[i] = slotInfo;
                OnAddItem?.Invoke(item, i, addNum);
                _freeCapacity--;
                num -= item.MaxNum;
                if (num <= 0)
                    return 0;
            }

            return num;
        }

        public int AddItem(T item, int index, int num)
        {
            if (index < 0 || index >= inventorySlotInfos.Length)
                return num;
            
            var slotInfo = inventorySlotInfos[index];
            int addNum;
            
            if (EqualityComparer<T>.Default.Equals(slotInfo.item, default))
            {
                addNum = Mathf.Min(num, item.MaxNum);
                slotInfo.item = item;
                slotInfo.num = addNum;
                inventorySlotInfos[index] = slotInfo;
                OnAddItem?.Invoke(item, index, addNum);
                _freeCapacity--;
                num -= item.MaxNum;
                return Mathf.Max(num, 0);
            }

            if (slotInfo.item.Name != item.Name)
                return num;
            
            var canAddNum = slotInfo.item.MaxNum - slotInfo.num;
            addNum = Mathf.Min(num, canAddNum);
            slotInfo.num += addNum;
            inventorySlotInfos[index] = slotInfo;
            OnAddItem?.Invoke(item, index, addNum);
            num -= canAddNum;
            return Mathf.Max(num, 0);
        }
        
        public int RemoveItem(T item, int num)
        {
            if (_freeCapacity == inventorySlotInfos.Length)
                return num;

            for (var i = inventorySlotInfos.Length - 1; i >= 0; i--)
            {
                var slotInfo = inventorySlotInfos[i];
                if (EqualityComparer<T>.Default.Equals(slotInfo.item, default))
                    continue;
                
                if (slotInfo.item.Name == item.Name && slotInfo.num > 0)
                {
                    var canRemoveNum = slotInfo.num;
                    var removeNum = Mathf.Min(num, canRemoveNum);
                    if (num < canRemoveNum)
                    {
                        slotInfo.num -= num;
                        inventorySlotInfos[i] = slotInfo;
                        OnRemoveItem?.Invoke(item, i, removeNum);
                        return 0;
                    }
                    else
                    {
                        slotInfo.item = default;
                        slotInfo.num = 0;
                        inventorySlotInfos[i] = slotInfo;
                        OnRemoveItem?.Invoke(item, i, removeNum);
                        _freeCapacity++;
                        num -= removeNum;
                    }
                }
            }

            return num;
        }

        public int RemoveItem(T item, int index, int num)
        {
            if (index < 0 || index >= inventorySlotInfos.Length)
                return num;
            
            var slotInfo = inventorySlotInfos[index];
            
            if (EqualityComparer<T>.Default.Equals(slotInfo.item, default))
                return num;
            
            if (slotInfo.item.Name != item.Name)
                return num;
            
            var canRemoveNum = slotInfo.num;
            var removeNum = Mathf.Min(num, canRemoveNum);
            if (num < canRemoveNum)
            {
                slotInfo.num -= num;
                inventorySlotInfos[index] = slotInfo;
                OnRemoveItem?.Invoke(item, index, removeNum);
                return 0;
            }
            else
            {
                slotInfo.item = default;
                slotInfo.num = 0;
                inventorySlotInfos[index] = slotInfo;
                OnRemoveItem?.Invoke(item, index, removeNum);
                _freeCapacity++;
                return num - removeNum;
            }
        }

        public int Capacity => inventorySlotInfos.Length;
        public bool HasItem(int index) => !EqualityComparer<T>.Default.Equals(inventorySlotInfos[index].item, default) &&
                                          inventorySlotInfos[index].num > 0;
        public InventorySlotInfo<T> GetInventorySlotInfo(int index) => inventorySlotInfos[index];
    }
}