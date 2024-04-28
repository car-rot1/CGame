namespace CGame
{
    public interface IDynamicUI
    {
        string Key { get; }
        void Open(object param);
        void Close(object param);
        void Show(object param);
        void Hide(object param);
    }
}