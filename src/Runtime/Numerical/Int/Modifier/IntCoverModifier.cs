using System;
using UnityEngine;

namespace CGame
{
    [Serializable]
    public sealed class IntCoverModifier : IntModifierBase
    {
        public override int Priority => priority;
        
        [SerializeField] public int coverValue;
        [SerializeField] public int priority;
        
        public IntCoverModifier(int coverValue, int priority = 0)
        {
            this.coverValue = coverValue;
            this.priority = priority;
        }

        public override int ModifierValue(int value)
        {
            return coverValue;
        }
    }
}