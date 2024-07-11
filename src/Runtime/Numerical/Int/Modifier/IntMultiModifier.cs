using System;
using UnityEngine;

namespace CGame
{
    [Serializable]
    public class IntMultiModifier : IntModifierBase
    {
        public override int Priority => priority;
        
        [SerializeField] public float multiValue;
        [SerializeField] public int priority;
        
        public IntMultiModifier(float multiValue, int priority = 0)
        {
            this.multiValue = multiValue;
            this.priority = priority;
        }

        public override int ModifierValue(int value)
        {
            return Mathf.RoundToInt(value * multiValue);
        }
    }
}