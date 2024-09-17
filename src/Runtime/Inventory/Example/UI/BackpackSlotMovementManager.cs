using UnityEngine.EventSystems;

namespace CGame
{
    public class BackpackSlotMovementManager : InventorySlotMovementManagerBase<IBackpackItem>
    {
        protected override bool BeginControllerSlot(int index, InventorySlotMovementBase<IBackpackItem> slotMovement, PointerEventData eventData)
        {
            if (index == -1 || !CurrentInventory.HasItem(index))
                return false;

            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    return base.BeginControllerSlot(index, slotMovement, eventData);
                case PointerEventData.InputButton.Right:
                    var info = CurrentInventory.GetInventorySlotInfo(index);
                    info.num /= 2;
                    slotMovement.Item = info.item;
                    slotMovement.Num = info.num;
                    CurrentInventory.RemoveItem(info.item, index, info.num);
                    return true;
                case PointerEventData.InputButton.Middle:
                    return base.BeginControllerSlot(index, slotMovement, eventData);
            }

            return base.BeginControllerSlot(index, slotMovement, eventData);
        }

        protected override bool EndControllerSlot(int index, InventorySlotMovementBase<IBackpackItem> slotMovement, PointerEventData eventData)
        {
            if (index == -1)
            {
                slotMovement.Clear();
                return true;
            }

            if (CurrentInventory.HasItem(index))
            {
                var info = CurrentInventory.GetInventorySlotInfo(index);
                if (info.item.Name != slotMovement.Item.Name)
                {
                    CurrentInventory.RemoveItem(info.item, index, info.num);
                    CurrentInventory.AddItem(slotMovement.Item, index, slotMovement.Num);
                    slotMovement.Clear();
                    slotMovement.Item = info.item;
                    slotMovement.Num = info.num;
                    return false;
                }
            }

            var num = CurrentInventory.AddItem(slotMovement.Item, index, slotMovement.Num);
            if (num <= 0)
            {
                slotMovement.Clear();
                return true;
            }

            slotMovement.Num = num;
            return false;
        }
    }
}