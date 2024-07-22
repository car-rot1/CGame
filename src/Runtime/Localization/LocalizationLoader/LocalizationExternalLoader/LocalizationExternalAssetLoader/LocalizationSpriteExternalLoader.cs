using System;
using System.IO;
using UnityEngine;

namespace CGame.Localization
{
    [Serializable]
    public class LocalizationSpriteExternalLoader : LocalizationAssetExternalLoaderBase
    {
        public override string Key => nameof(LocalizationSpriteExternalLoader);

        public LocalizationSpriteExternalLoader()
        {
            ExternalPath = "../Language/Image";
        }

        protected override void RefreshExternalResource(string language)
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
        }
        
        public string GetImageFileNameWithoutExtension(string path) => IsImageFile(path) ? Path.GetFileNameWithoutExtension(path) : null;
        public bool IsImageFile(string path) => Path.GetExtension(path) is ".jpg" or ".png" or ".bmp";
    }
}