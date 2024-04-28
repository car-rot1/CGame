namespace CGame
{
    public class ReferencePool<T> : PoolBase<T> where T : class, new()
    {
        protected ReferencePool(int capacity = 50) : base(capacity) { }
        
        private static ReferencePool<T> _referencePool;
        
        public static ReferencePool<T> GetPool(int capacity = 50)
        {
            if (_referencePool == null)
            {
                _referencePool = new ReferencePool<T>(capacity);
            }
            else
            {
                _referencePool.capacity = capacity;
            }
            return _referencePool;
        }
        
        protected override T CreateHandle() => new();
    }
}