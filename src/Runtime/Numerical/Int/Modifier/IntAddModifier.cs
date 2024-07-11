using System;
using UnityEngine;

namespace CGame
{
    [Serializable]
    public sealed class IntAddModifier : IntModifierBase
    {
        public override int Priority => priority;
        
        [SerializeField] public int addValue;
        [SerializeField] public int priority;
        
        public IntAddModifier(int addValue, int priority = 0)
        {
            this.addValue = addValue;
            this.priority = priority;
        }
        
        public override int ModifierValue(int value)
        {
            return value + addValue;
        }
    }
}