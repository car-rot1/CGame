using System;

namespace CGame
{
    [Obsolete("该对象池已废弃，理解起来很奇怪，请使用ReferencePool")]
    public class NewReferencePool<T> : NewPoolBase<T> where T : class, new()
    {
        public NewReferencePool(int capacity = 50) : base(capacity) { }
        
        protected override T CreateHandle() => new();
    }
}