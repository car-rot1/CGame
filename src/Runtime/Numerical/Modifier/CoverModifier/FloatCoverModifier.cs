using System;
using UnityEngine;

namespace CGame
{
    [Serializable]
    public class FloatCoverModifier : CoverModifierBase<float>
    {
        [SerializeField] public float coverValue;
        
        public FloatCoverModifier(float coverValue)
        {
            this.coverValue = coverValue;
        }

        public override float ModifierValue(float value)
        {
            return coverValue;
        }
    }
}