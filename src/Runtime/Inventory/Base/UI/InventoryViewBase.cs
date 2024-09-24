using UnityEngine;

namespace CGame
{
    public abstract class InventoryViewBase<T> : MonoBehaviour where T : IInventoryItem
    {
        public InventorySlotBase<T>[] slots;
        public InventoryBase<T> CurrentInventory { get; private set; }
        
        public virtual void Init(InventoryBase<T> inventory)
        {
            CurrentInventory = inventory;

            for (var i = 0; i < CurrentInventory.Capacity; i++)
            {
                if (!CurrentInventory.HasItem(i))
                    continue;
                
                var inventorySlotInfo = CurrentInventory.GetInventorySlotInfo(i);
                slots[i].SetItem(inventorySlotInfo.item, inventorySlotInfo.num);
            }

            CurrentInventory.OnAddItem += OnAddItem;
            CurrentInventory.OnRemoveItem += OnRemoveItem;
        }

        private void OnAddItem(T item, int index, int num)
        {
            var info = CurrentInventory.GetInventorySlotInfo(index);
            slots[index].SetItem(info.item, info.num);
        }

        private void OnRemoveItem(T item, int index, int num)
        {
            if (!CurrentInventory.HasItem(index))
            {
                slots[index].Clear();
                return;
            }
            var info = CurrentInventory.GetInventorySlotInfo(index);
            slots[index].SetItem(info.item, info.num);
        }

        public virtual void DeInit()
        {
            foreach (var slot in slots)
            {
                slot.Clear();
            }

            CurrentInventory.OnAddItem -= OnAddItem;
            CurrentInventory.OnRemoveItem -= OnRemoveItem;
        }
    }
}