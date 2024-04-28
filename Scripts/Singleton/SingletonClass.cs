using System;

namespace CGame
{
    public class SingletonClass<T> where T : SingletonClass<T>, new()
    {
        protected SingletonClass() { }
        private static readonly Lazy<T> LazyInstance = new(GetInstance);
        public static T Instance => LazyInstance.Value;
        private static T GetInstance()
        {
            var result = new T();
            result.Init();
            return result;
        }
        protected virtual void Init() { }
    }
}