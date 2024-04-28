using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace CGame
{
    public interface ICustomAsyncTask
    {
        IEnumerator AsIEnumerator();
    }
    
    public abstract class AssetsLoaderBase<TAsyncTask> where TAsyncTask : ICustomAsyncTask
    {
        public abstract bool TryLoadAsset([NotNull] string path, out TAsyncTask customAsyncTask);

        public abstract bool TryReadAsset([NotNull] string path, out GameObject asset);
    }
}