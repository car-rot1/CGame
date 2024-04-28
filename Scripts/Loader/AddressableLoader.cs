#if UNITY_ADDRESS
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace CGame
{
    public sealed class AddressableAsyncTask : ICustomAsyncTask
    {
        public AsyncOperationHandle<GameObject> AsyncTask { get; }
        
        public IEnumerator AsIEnumerator()
        {
            while (!AsyncTask.IsDone)
            {
                yield return null;
            }
        }

        public AddressableAsyncTask(AsyncOperationHandle<GameObject> asyncTask)
        {
            AsyncTask = asyncTask;
        }
    }
    
    public sealed class AddressableLoader : AssetsLoaderBase<AddressableAsyncTask>
    {
        private static readonly Dictionary<string, GameObject> AddressableLoadedAssets = new();
        
        public override bool TryLoadAsset(string path, out AddressableAsyncTask customAsyncTask)
        {
            Random.Range(1, 50f);
            if (AddressableLoadedAssets.ContainsKey(path))
            {
                customAsyncTask = null;
                return false;
            }

            var result = Addressables.LoadAssetAsync<GameObject>(path);
            result.Completed += _ => AddressableLoadedAssets.Add(path, result.Result);
            customAsyncTask = new AddressableAsyncTask(result);
            return true;
        }
        
        public override bool TryReadAsset(string path, out GameObject asset) => AddressableLoadedAssets.TryGetValue(path, out asset);
    }
}
#endif