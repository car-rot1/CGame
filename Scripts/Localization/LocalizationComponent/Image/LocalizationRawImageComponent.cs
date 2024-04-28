using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CGame.Localization
{
    [RequireComponent(typeof(RawImage))]
    [AddComponentMenu("Localization/LocalizationRawImageComponent")]
    public class LocalizationRawImageComponent : LocalizationComponentBase
    {
        [SerializeField] private string id;
        
        public string Id
        {
            get => string.IsNullOrWhiteSpace(id) ? gameObject.name : id;
            set
            {
                if (value.Equals(id))
                    return;
                id = value;
                LanguageChange(localizationSystem.Language);
            }
        }
        
        [SerializeField] private bool autoSetNativeSize;
        
        [Serializable]
        private struct TextureInfo
        {
            public string language;
            public Texture texture;
        }
        [SerializeField] private List<TextureInfo> customTextures = new();
        private Dictionary<string, Texture> _textureInfoDic;
        private Dictionary<string, Texture> TextureInfoDic => _textureInfoDic ??= customTextures.ToDictionary(info => info.language, info => info.texture);
        
        private Texture _defaultTexture;
        private Texture DefaultTexture
        {
            get
            {
                if (_defaultTexture == null)
                    _defaultTexture = TargetImage.texture;
                return _defaultTexture;
            }
        }
        
        private RawImage _targetImage;
        public RawImage TargetImage
        {
            get
            {
                if (_targetImage == null)
                    _targetImage = GetComponent<RawImage>();
                return _targetImage;
            }
        }
        
        protected override void LanguageChange(string language)
        {
            TargetImage.texture = ImageToLocal(DefaultTexture);
            if (autoSetNativeSize)
                TargetImage.SetNativeSize();
        }
        
        private Texture ImageToLocal(Texture texture)
        {
            if (TextureInfoDic.TryGetValue(localizationSystem.Language, out var customTexture) && customTexture != null)
                return customTexture;
            
            var localTexture = localizationSystem.GetTexture(Id);
            return localTexture != null ? localTexture : texture;
        }
    }
}