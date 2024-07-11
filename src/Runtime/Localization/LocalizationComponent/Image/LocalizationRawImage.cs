using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CGame.Localization
{
    [AddComponentMenu("Localization/LocalizationRawImage")]
    public class LocalizationRawImage : RawImage
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
                LanguageChange(Local.Language);
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
                    _defaultTexture = texture;
                return _defaultTexture;
            }
        }
        
        private LocalizationSystem _local;
        private LocalizationSystem Local => _local ??= LocalizationSystem.Instance;

        protected override void Awake()
        {
            LanguageChange(Local.Language);
            Local.OnLanguageChange += LanguageChange;
        }

        private void LanguageChange(string language)
        {
            if (!Application.isPlaying)
                return;

            texture = ImageToLocal(DefaultTexture);
            if (autoSetNativeSize)
                SetNativeSize();
        }
        
        private Texture ImageToLocal(Texture texture)
        {
            if (TextureInfoDic.TryGetValue(Local.Language, out var customTexture) && customTexture != null)
                return customTexture;

            var sprite = (Sprite)Local.GetAsset(nameof(LocalizationSpriteLoader), Id);
            if (sprite == null)
                return null;
            var localTexture = sprite.GetPartTexture();
            return localTexture != null ? localTexture : texture;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Local.OnLanguageChange -= LanguageChange;
        }
    }
}