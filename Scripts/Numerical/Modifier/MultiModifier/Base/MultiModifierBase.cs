using System;

namespace CGame
{
    [Serializable]
    public abstract class MultiModifierBase<T> : ModifierBase<T>
    {
        public override int Priority => 1;
    }
}