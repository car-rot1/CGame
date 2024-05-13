using UnityEngine;

namespace CGame
{
    public sealed class RecursiveDivisionMapGenerate : MapGenerateBase
    {
        public RecursiveDivisionMapGenerate(int? seed = null) : base(seed)
        {
            
        }

        protected override MapInfo GenerateMap(Vector2Int start, int width, int height)
        {
            var mapInfo = new MapInfo(width, height, start);
            
            for (var i = 0; i < width; i++)
            for (var j = 0; j < height; j++)
            {
                mapInfo.AddRoom(new RoomInfo(new Vector2Int(i, j), Color.white));
            }

            RecursiveDivision(mapInfo, new RectInt(0, 0, width - 1, height - 1));

            return mapInfo.RefreshDepth();
        }

        private void RecursiveDivision(MapInfo mapInfo, RectInt range)
        {
            if (range.width <= 0 || range.height <= 0)
            {
                for (var i = 0; i < range.height; i++)
                {
                    var point0 = new Vector2Int(range.xMin, range.yMin + i);
                    var point1 = new Vector2Int(range.xMin, range.yMin + i + 1);
                    
                    mapInfo.ConnectRoom(point0, (point1 - point0).ToDirection());
                }

                for (var i = 0; i < range.width; i++)
                {
                    var point0 = new Vector2Int(range.xMin + i, range.yMin);
                    var point1 = new Vector2Int(range.xMin + i + 1, range.yMin);
                
                    mapInfo.ConnectRoom(point0, (point1 - point0).ToDirection());
                }
                return;
            }
            
            var x = Random.Range(range.xMin, range.xMax);
            var y = Random.Range(range.yMin, range.yMax);

            var points = new Vector2Int[]
            {
                new(Random.Range(range.xMin, x), y),
                new(Random.Range(x + 1, range.xMax + 1), y),
                new(x, Random.Range(range.yMin, y)),
                new(x, Random.Range(y + 1, range.yMax + 1)),
            };
            
            foreach (var i in RandomExtension.MultiRange(0, points.Length, 3))
            {
                if (i < 2)
                {
                    var point0 = points[i];
                    var point1 = points[i] + Vector2Int.up;
                    
                    mapInfo.ConnectRoom(point0, (point1 - point0).ToDirection());
                }
                else
                {
                    var point0 = points[i];
                    var point1 = points[i] + Vector2Int.right;
                    
                    mapInfo.ConnectRoom(point0, (point1 - point0).ToDirection());
                }
            }
            
            RecursiveDivision(mapInfo, new RectInt(range.xMin, y + 1, x - range.xMin, range.yMax - (y + 1)));
            RecursiveDivision(mapInfo, new RectInt(x + 1, y + 1, range.xMax - (x + 1), range.yMax - (y + 1)));
            RecursiveDivision(mapInfo, new RectInt(range.xMin, range.yMin, x - range.xMin, y - range.yMin));
            RecursiveDivision(mapInfo, new RectInt(x + 1, range.yMin, range.xMax - (x + 1), y - range.yMin));
        }
    }
}