using System;

namespace CGame
{
    [Serializable]
    public abstract class ModifierBase<T>
    {
        public abstract int Priority { get; }
        public abstract T ModifierValue(T value);
    }
}