using System;
using UnityEngine;

namespace CGame
{
    [Serializable]
    public sealed class IntCoverModifier : CoverModifierBase<int>
    {
        [SerializeField] public int coverValue;
        
        public IntCoverModifier(int coverValue)
        {
            this.coverValue = coverValue;
        }

        public override int ModifierValue(int value)
        {
            return coverValue;
        }
    }
}