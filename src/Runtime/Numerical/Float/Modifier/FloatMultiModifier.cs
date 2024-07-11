using System;
using UnityEngine;

namespace CGame
{
    [Serializable]
    public class FloatMultiModifier : FloatModifierBase
    {
        public override int Priority => priority;
        
        [SerializeField] public float multiValue;
        [SerializeField] public int priority;
        
        public FloatMultiModifier(float multiValue, int priority = 0)
        {
            this.multiValue = multiValue;
            this.priority = priority;
        }

        public override float ModifierValue(float value)
        {
            return value * multiValue;
        }
    }
}