using System;
using System.IO;
using UnityEngine;

namespace CGame.Localization
{
    [Serializable]
    public class LocalizationSpriteLoader : LocalizationAssetLoaderBase
    {
        public override string Key => nameof(LocalizationSpriteLoader);

        public LocalizationSpriteLoader()
        {
            InternalPath = "Language/Sprite";
            InternalLoadType = InternalLoadType.Resource;
            ExternalPath = "../Language/Image";
        }

        protected override bool RefreshExternalResource(LocalizationSystem localizationSystem, string language)
        {
            var folderPath = ExternalPath + '/' + language;
            if (Directory.Exists(folderPath))
            {
                foreach (var file in Directory.GetFiles(folderPath))
                {
                    if (!IsImageFile(file))
                        continue;
                        
                    var imgByte = File.ReadAllBytes(file);
                    var texture = new Texture2D(128, 128);
                    texture.LoadImage(imgByte);
                    AllResource[Path.GetFileNameWithoutExtension(file)] = texture.ToSprite();
                }
            }
            return base.RefreshExternalResource(localizationSystem, language);
        }

        protected override bool RefreshInternalResource(LocalizationSystem localizationSystem, string language)
        {
            switch (InternalLoadType)
            {
                case InternalLoadType.Resource:
                {
                    var path = InternalPath + '/' + language;
                    var imageSo = Resources.Load<LanguageSpriteSO>(path);
                    if (imageSo == null)
                        break;
                    foreach (var languageImageInfo in imageSo.languageImageInfos)
                        AllResource[languageImageInfo.id] = languageImageInfo.sprite;
                    return true;
                }
                case InternalLoadType.Addressable:
                    return true;
                case InternalLoadType.Yooasset:
                    return true;
            }
            return false;
        }
        
        public string GetImageFileNameWithoutExtension(string path) => IsImageFile(path) ? Path.GetFileNameWithoutExtension(path) : null;
        public bool IsImageFile(string path) => Path.GetExtension(path) is ".jpg" or ".png" or ".bmp";
    }
}