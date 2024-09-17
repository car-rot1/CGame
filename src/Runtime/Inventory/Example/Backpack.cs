namespace CGame
{
    public class Backpack : InventoryBase<IBackpackItem>
    {
        public Backpack(int capacity) : base(capacity)
        {
        }
    }
}