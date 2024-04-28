using System.Collections.Generic;

namespace CGame
{
    public class ListPool<T> : ReferencePool<List<T>>
    {
        protected override void ReleaseHandle(List<T> obj) => obj.Clear();
    }
}
