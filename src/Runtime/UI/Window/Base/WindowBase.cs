using CGame;
using UnityEngine;

namespace CGame
{
    public abstract class WindowBase : MonoBehaviour, IDynamicUI
    {
        public abstract string Key { get; }

        public virtual void Open(object param)
        {
        }

        public virtual void Close(object param)
        {
        }

        public virtual void Show(object param)
        {
        }

        public virtual void Hide(object param)
        {
        }
    }

    public abstract class WindowBase<TOpenParam> : WindowBase
    {
        public sealed override void Open(object param) => Open((TOpenParam)param);

        public virtual void Open(TOpenParam param)
        {
        }
    }

    public abstract class WindowBase<TOpenParam, TCloseParam> : WindowBase
    {
        public sealed override void Open(object param) => Open((TOpenParam)param);

        public virtual void Open(TOpenParam param)
        {
        }

        public sealed override void Close(object param) => Close((TCloseParam)param);

        public virtual void Close(TCloseParam param)
        {
        }
    }

    public abstract class WindowBase<TOpenParam, TCloseParam, TShowParam> : WindowBase
    {
        public sealed override void Open(object param) => Open((TOpenParam)param);

        public virtual void Open(TOpenParam param)
        {
        }

        public sealed override void Close(object param) => Close((TCloseParam)param);

        public virtual void Close(TCloseParam param)
        {
        }

        public sealed override void Show(object param) => Show((TShowParam)param);

        public virtual void Show(TShowParam param)
        {
        }
    }

    public abstract class WindowBase<TOpenParam, TCloseParam, TShowParam, THideParam> : WindowBase
    {
        public sealed override void Open(object param) => Open((TOpenParam)param);

        public virtual void Open(TOpenParam param)
        {
        }

        public sealed override void Close(object param) => Close((TCloseParam)param);

        public virtual void Close(TCloseParam param)
        {
        }

        public sealed override void Show(object param) => Show((TShowParam)param);

        public virtual void Show(TShowParam param)
        {
        }

        public sealed override void Hide(object param) => Hide((THideParam)param);

        public virtual void Hide(THideParam param)
        {
        }
    }
}