using System;
using UnityEngine;

namespace CGame
{
    [Serializable]
    public class FloatCoverModifier : FloatModifierBase
    {
        public override int Priority => priority;
        
        [SerializeField] public float coverValue;
        [SerializeField] public int priority;
        
        public FloatCoverModifier(float coverValue, int priority = 0)
        {
            this.coverValue = coverValue;
            this.priority = priority;
        }

        public override float ModifierValue(float value)
        {
            return coverValue;
        }
    }
}