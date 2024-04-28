using System;
using UnityEngine;

namespace CGame
{
    [Serializable]
    public sealed class FloatAddModifier : AddModifierBase<float>
    {
        [SerializeField] public float addValue;
        
        public FloatAddModifier(float addValue)
        {
            this.addValue = addValue;
        }
        
        public override float ModifierValue(float value)
        {
            return value + addValue;
        }
    }
}