#if UNITY_YOOASSET
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace CGame
{
    public sealed class YooassetAsyncTask : ICustomAsyncTask
    {
        public AssetHandle AsyncTask { get; }
        
        public IEnumerator AsIEnumerator()
        {
            yield return AsyncTask;
        }

        public YooassetAsyncTask(AssetHandle asyncTask)
        {
            AsyncTask = asyncTask;
        }
    }
    
    public class YooassetLoader : AssetsLoaderBase<YooassetAsyncTask>
    {
        private static readonly Dictionary<string, GameObject> YooassetLoadedAssets = new();
        
        public override bool TryLoadAsset(string path, out YooassetAsyncTask customAsyncTask)
        {
            if (YooassetLoadedAssets.ContainsKey(path))
            {
                customAsyncTask = null;
                return false;
            }
            
            var result = YooAssets.LoadAssetAsync<GameObject>(path);
            result.Completed += assetHandle => YooassetLoadedAssets.Add(path, assetHandle.AssetObject as GameObject);
            customAsyncTask = new YooassetAsyncTask(result);
            return true;
        }

        public override bool TryReadAsset(string path, out GameObject asset) => YooassetLoadedAssets.TryGetValue(path, out asset);
    }
}
#endif
