using System;
using UnityEngine;

namespace CGame
{
    [Serializable]
    public class FloatMultiModifier : MultiModifierBase<float>
    {
        [SerializeField] public float multiValue;
        
        public FloatMultiModifier(float multiValue)
        {
            this.multiValue = multiValue;
        }

        public override float ModifierValue(float value)
        {
            return value * multiValue;
        }
    }
}