using UnityEngine;

namespace CGame
{
    public static class Texture2DExtension
    {
        public static Sprite ToSprite(this Texture2D self) => Sprite.Create(self, new Rect(0, 0, self.width, self.height), new Vector2(0.5f, 0.5f));
    }
}