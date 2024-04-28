using System.Linq;
using UnityEngine;

namespace CGame
{
    public class BackpackView : MonoBehaviour
    {
        public BackpackItemSlot[] slots;
        private Backpack _currentBackpack;

        private void OnEnable()
        {
            var backpack = new Backpack();
            if (_currentBackpack == backpack)
                return;

            if (_currentBackpack != null)
            {
                _currentBackpack.OnAddItem -= AddItem;
                _currentBackpack.OnRemoveItem -= RemoveItem;
            }

            _currentBackpack = backpack;

            var j = 0;
            var allItem = _currentBackpack.AllItem;
            foreach (var (item, num) in allItem)
            {
                var tempNum = num;
                while (tempNum > 0)
                {
                    tempNum = slots[j++].SetItem(item, tempNum);
                }
            }

            for (var i = j; i < slots.Length; i++)
                slots[i].Clear();

            _currentBackpack.OnAddItem += AddItem;
            _currentBackpack.OnRemoveItem += RemoveItem;
        }

        private void AddItem(IBackpackItem item, int num)
        {
            if (num <= 0)
                return;

            var canAddSlots =
                slots.Where(slot => slot.Item != null && slot.Item.Name == item.Name && slot.Num < item.MaxNum);
            foreach (var canAddSlot in canAddSlots)
            {
                num = canAddSlot.AddNum(num);
                if (num <= 0)
                    return;
            }

            var canExtraSlots = slots.Where(slot => slot.Item == null);
            foreach (var canExtraSlot in canExtraSlots)
            {
                num = canExtraSlot.SetItem(item, num);
                if (num <= 0)
                    return;
            }
        }

        private void RemoveItem(IBackpackItem item, int num)
        {
            var canRemoveSlots = slots.Where(slot => slot.Item != null && slot.Item.Name == item.Name).Reverse()
                .OrderBy(slot => slot.Num);
            foreach (var canRemoveSlot in canRemoveSlots)
            {
                num = canRemoveSlot.RemoveNum(num);
                if (num <= 0)
                    return;
            }
        }

        private void OnDestroy()
        {
            _currentBackpack.OnAddItem -= AddItem;
            _currentBackpack.OnRemoveItem -= RemoveItem;
        }
    }
}
