using System.Collections.Generic;
using UnityEngine;

namespace CGame
{
    public struct TileInfo
    {
        public static readonly Vector2Int[] Directions =
        {
            new(0, 1),
            new(1, 0),
            new(0, -1),
            new(-1, 0),
        };
        
        public Vector2Int position;
        public readonly List<int> currentKey;
        public int currentWeight => currentKey.Count;

        public TileInfo(Vector2Int position)
        {
            this.position = position;
            currentKey = new List<int>(9) { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        }
    }
    
    public class WaveCollapse
    {
        public int[,] Test(RectInt range)
        {
            var result = new int[range.width + 1, range.height + 1];
            
            var allTile = new List<TileInfo>();
            var allTileDic = new Dictionary<Vector2Int, TileInfo>();
            for (var i = range.xMin; i <= range.xMax; i++)
            for (var j = range.yMin; j <= range.yMax; j++)
            {
                var tile = new TileInfo(new Vector2Int(i, j));
                allTile.Add(tile);
                allTileDic.Add(tile.position, tile);
            }

            while (allTile.Count > 0)
            {
                allTile.Sort((tile0, tile1) => tile0.currentWeight - tile1.currentWeight);
                var randomTile = allTile[0];
                var randomKey = randomTile.currentKey.RandomItem();

                result[randomTile.position.x, randomTile.position.y] = randomKey;

                var xMin = randomTile.position.x / 3;
                var yMin = randomTile.position.y / 3;
                
                for (var i = 0; i < 3; i++)
                for (var j = 0; j < 3; j++)
                {
                    var targetPoint = new Vector2Int(xMin * 3 + i, yMin * 3 + j);
                    allTileDic[targetPoint].currentKey.Remove(randomKey);
                }
                
                foreach (var direction in TileInfo.Directions)
                {
                    var targetPoint = randomTile.position + direction;
                    while (range.ContainsIncludeBorder(targetPoint))
                    {
                        var targetTile = allTileDic[targetPoint];
                        targetTile.currentKey.Remove(randomKey);
                        targetPoint += direction;
                    }
                }
                allTile.RemoveAt(0);
            }

            return result;
        }
    }
}