using UnityEngine;
using UnityEngine.Events;

namespace CGame.Localization
{
    public abstract class LocalizationAssetUnityEventBase<TAsset, TLoader> : LocalizationAssetBase<TAsset, TLoader> where TAsset : Object where TLoader : LocalizationAssetExternalLoaderBase
    {
        [SerializeField] private UnityEvent<TAsset> OnUpdateAsset;

        protected sealed override void UpdateAsset(TAsset asset)
        {
            OnUpdateAsset?.Invoke(asset);
        }
    }
}