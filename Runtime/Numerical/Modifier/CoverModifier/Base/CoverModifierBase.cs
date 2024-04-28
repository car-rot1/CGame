using System;

namespace CGame
{
    [Serializable]
    public abstract class CoverModifierBase<T> : ModifierBase<T>
    {
        public override int Priority => 2;
    }
}