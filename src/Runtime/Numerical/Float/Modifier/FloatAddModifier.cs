using System;
using UnityEngine;

namespace CGame
{
    [Serializable]
    public sealed class FloatAddModifier : FloatModifierBase
    {
        public override int Priority => priority;
        
        [SerializeField] public float addValue;
        [SerializeField] public int priority;
        
        public FloatAddModifier(float addValue, int priority = 0)
        {
            this.addValue = addValue;
            this.priority = priority;
        }
        
        public override float ModifierValue(float value)
        {
            return value + addValue;
        }
    }
}