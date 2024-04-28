using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace CGame
{
    public sealed class ResourcesAsyncTask : ICustomAsyncTask
    {
        public ResourceRequest AsyncTask { get; }
        
        public IEnumerator AsIEnumerator()
        {
            while (!AsyncTask.isDone)
            {
                yield return null;
            }
        }

        public ResourcesAsyncTask(ResourceRequest asyncTask)
        {
            AsyncTask = asyncTask;
        }
    }
    
    public sealed class ResourcesLoader : AssetsLoaderBase<ResourcesAsyncTask>
    {
        private static readonly Dictionary<string, GameObject> ResourcesLoadedAssets = new();
        
        public override bool TryLoadAsset(string path, out ResourcesAsyncTask customAsyncTask)
        {
            if (ResourcesLoadedAssets.ContainsKey(path))
            {
                customAsyncTask = null;
                return false;
            }

            var result = Resources.LoadAsync<GameObject>(path);
            result.completed += _ => ResourcesLoadedAssets.Add(path, result.asset as GameObject);
            customAsyncTask = new ResourcesAsyncTask(result);
            return true;
        }

        public override bool TryReadAsset(string path, out GameObject asset) => ResourcesLoadedAssets.TryGetValue(path, out asset);
    }
}