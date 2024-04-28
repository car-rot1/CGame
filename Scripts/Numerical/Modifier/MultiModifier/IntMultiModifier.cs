using System;
using UnityEngine;

namespace CGame
{
    [Serializable]
    public class IntMultiModifier : MultiModifierBase<int>
    {
        [SerializeField] public float multiValue;
        
        public IntMultiModifier(float multiValue)
        {
            this.multiValue = multiValue;
        }

        public override int ModifierValue(int value)
        {
            return Mathf.RoundToInt(value * multiValue);
        }
    }
}