using System;
using UnityEngine;

namespace CGame
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class RequiredInterfaceMonoAttribute : PropertyAttribute
    {
        public readonly Type interfaceType;

        public RequiredInterfaceMonoAttribute(Type interfaceType)
        {
            this.interfaceType = interfaceType;
        }
    }
}