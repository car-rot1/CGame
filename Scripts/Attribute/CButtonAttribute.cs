using System;

namespace CGame
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CButtonAttribute : Attribute
    {
        public readonly string name;
        public CButtonAttribute(string name = null) => this.name = name;
    }
}