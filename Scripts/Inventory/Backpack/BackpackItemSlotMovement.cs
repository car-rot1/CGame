namespace CGame
{
    public class BackpackItemSlotMovement : ItemSlotMovementBase<IBackpackItem>
    {
        protected override void SlotCombine(ItemSlotBase<IBackpackItem> sourceSlot, ItemSlotBase<IBackpackItem> targetSlot)
        {
            if (targetSlot.Item == null)
            {
                targetSlot.SetItem(sourceSlot.Item, sourceSlot.Num);
                sourceSlot.Clear();
            }
            else if (targetSlot.Item.Name == sourceSlot.Item.Name)
            {
                var num = targetSlot.AddNum(sourceSlot.Num);
                sourceSlot.RemoveNum(sourceSlot.Num - num);
            }
        }
    }
}