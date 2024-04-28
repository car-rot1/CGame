using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CGame.Localization
{
    [AddComponentMenu("Localization/LocalizationImage")]
    public class LocalizationImage : Image
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
                    _defaultSprite = sprite;
                return _defaultSprite;
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

            sprite = ImageToLocal(DefaultSprite);
            if (autoSetNativeSize)
                SetNativeSize();
        }
        
        private Sprite ImageToLocal(Sprite sprite)
        {
            if (SpriteInfoDic.TryGetValue(Local.Language, out var customSprite) && customSprite != null)
                return customSprite;
            
            var localSprite = Local.GetSprite(Id);
            return localSprite != null ? localSprite : sprite;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Local.OnLanguageChange -= LanguageChange;
        }
    }
}