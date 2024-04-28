using UnityEngine;

namespace CGame
{
    public interface IInventoryItem
    {
        string Name { get; }
        Sprite Sprite { get; }
        int MaxNum { get; }
    }
}