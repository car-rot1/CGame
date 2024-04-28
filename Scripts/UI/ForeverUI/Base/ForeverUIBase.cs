using CGame;
using UnityEngine;

public abstract class ForeverUIBase : MonoBehaviour, IDynamicUI
{
    public abstract string Key { get; }
    public virtual void Open(object param) { }
    public virtual void Close(object param) { }
    public virtual void Show(object param) { }
    public virtual void Hide(object param) { }
    public virtual void Close() { }
}

public abstract class ForeverUIBase<TParam> : WindowBase
{
    public sealed override void Open(object param) => Open((TParam)param);
    public virtual void Open(TParam param) { }
}