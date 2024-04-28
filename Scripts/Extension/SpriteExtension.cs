using UnityEngine;

namespace CGame
{
    public static class SpriteExtension
    {
        public static Texture2D GetPartTexture(this Sprite self)
        {
            var width = (int)self.rect.width;
            var height = (int)self.rect.height;
            var texture2D = new Texture2D(width, height, self.texture.format, false);
            
            Graphics.CopyTexture(self.texture, 0, 0, (int)self.rect.x,
                    (int)self.rect.y, width, height, texture2D, 0, 0, 0, 0);
#if UNITY_EDITOR
                texture2D.alphaIsTransparency = self.texture.alphaIsTransparency;
#endif
                texture2D.filterMode = self.texture.filterMode;
            
            return texture2D;
        }
    }
}