using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CGame.Localization
{
    [RequireComponent(typeof(Image))]
    [AddComponentMenu("Localization/LocalizationImageComponent")]
    public class LocalizationImageComponent : LocalizationComponentBase
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
        private struct SpriteInfo
        {
            public string language;
            public Sprite sprite;
        }
        [SerializeField] private List<SpriteInfo> customSprites = new();
        private Dictionary<string, Sprite> _spriteInfoDic;
        private Dictionary<string, Sprite> SpriteInfoDic => _spriteInfoDic ??= customSprites.ToDictionary(info => info.language, info => info.sprite);
        
        private Sprite _defaultSprite;
        private Sprite DefaultSprite
        {
            get
            {
                if (_defaultSprite == null)
                    _defaultSprite = TargetImage.sprite;
                return _defaultSprite;
            }
        }
        
        private Image _targetImage;
        public Image TargetImage
        {
            get
            {
                if (_targetImage == null)
                    _targetImage = GetComponent<Image>();
                return _targetImage;
            }
        }

        protected override void LanguageChange(string language)
        {
            TargetImage.sprite = ImageToLocal(DefaultSprite);
            if (autoSetNativeSize)
                TargetImage.SetNativeSize();
        }
        
        private Sprite ImageToLocal(Sprite sprite)
        {
            if (SpriteInfoDic.TryGetValue(localizationSystem.Language, out var customSprite) && customSprite != null)
                return customSprite;
            
            var localSprite = (Sprite)localizationSystem.GetAsset(nameof(LocalizationSpriteLoader), Id);
            return localSprite != null ? localSprite : sprite;
        }
    }
}
