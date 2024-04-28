using System;
using UnityEngine;

namespace CGame
{
    [Serializable]
    public sealed class IntAddModifier : AddModifierBase<int>
    {
        [SerializeField] public int addValue;
        
        public IntAddModifier(int addValue)
        {
            this.addValue = addValue;
        }
        
        public override int ModifierValue(int value)
        {
            return value + addValue;
        }
    }
}