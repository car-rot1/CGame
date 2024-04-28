using System;

namespace CGame
{
    [Serializable]
    public abstract class AddModifierBase<T> : ModifierBase<T>
    {
        public override int Priority => 0;
    }
}