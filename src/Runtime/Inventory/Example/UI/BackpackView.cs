namespace CGame
{
    public class BackpackView : InventoryViewBase<IBackpackItem>, IDynamicUI
    {
        public string Key => nameof(BackpackView);
    
        public void Open(object param)
        {
            Init((Backpack)param);
        }

        public void Close(object param)
        {
            DeInit();
        }

        public void Show(object param)
        {
            Init((Backpack)param);
        }

        public void Hide(object param)
        {
            DeInit();
        }
    }
}