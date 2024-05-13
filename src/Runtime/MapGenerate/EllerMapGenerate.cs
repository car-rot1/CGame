using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CGame
{
    public sealed class EllerMapGenerate : MapGenerateBase
    {
        private readonly Vector2Int _horizontalNumRange;
        private readonly Vector2Int _verticalNumRange;
        
        public EllerMapGenerate(Vector2Int horizontalNumRange, Vector2Int verticalNumRange, int? seed = null) : base(seed)
        {
            _horizontalNumRange = horizontalNumRange;
            _verticalNumRange = verticalNumRange;
        }

        protected override MapInfo GenerateMap(Vector2Int start, int width, int height)
        {
            var roomList = new List<HashSet<Vector2Int>>(width);
            
            var mapInfo = new MapInfo(width, height, start);
            
            for (var i = 0; i < width; i++)
            for (var j = 0; j < height; j++)
            {
                mapInfo.AddRoom(new RoomInfo(new Vector2Int(i, j), Color.white));
            }
            
            for (var i = 0; i < height - 1; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    var point = new Vector2Int(j, i);
                    if (roomList.FindIndex(hashSet => hashSet.Contains(point)) == -1)
                        roomList.Add(new HashSet<Vector2Int> { point });
                }
                var hNum = Mathf.Clamp(Random.Range(_horizontalNumRange.x, _horizontalNumRange.y + 1), 0, width);
                var randomXs = RandomExtension.MultiRange(0, width - 1, hNum);
                foreach (var randomX in randomXs)
                {
                    var point0 = new Vector2Int(randomX, i);
                    var point1 = new Vector2Int(randomX + 1, i);
                    
                    var point0Index = roomList.FindIndex(hashSet => hashSet.Contains(point0));
                    var point1Index = roomList.FindIndex(hashSet => hashSet.Contains(point1));

                    if (point0Index != point1Index)
                    {
                        roomList[point0Index].UnionWith(roomList[point1Index]);
                        roomList.RemoveAt(point1Index);

                        mapInfo.ConnectRoom(point0, (point1 - point0).ToDirection());
                    }
                }
                foreach (var hashSet in roomList)
                {
                    if (hashSet.Count > 0)
                    {
                        var rowList = hashSet.Where(p => p.y == i).ToList();
                        var lastPoint = rowList.RandomItem();
                        hashSet.ExceptWith(rowList);
                        var vNum = Mathf.Clamp(Random.Range(_verticalNumRange.x, _verticalNumRange.y + 1), 1, height - 1 - i);
                        for (var j = 1; j <= vNum; j++)
                        {
                            var point = lastPoint + new Vector2Int(0, 1);
                            if (!hashSet.Contains(point))
                            {
                                hashSet.Add(point);
                                mapInfo.ConnectRoom(point, (lastPoint - point).ToDirection());
                            }
                            lastPoint = point;
                        }
                    }
                }
            }

            for (var i = 0; i < width; i++)
            {
                var point = new Vector2Int(i, height - 1);
                if (roomList.FindIndex(hashSet => hashSet.Contains(point)) == -1)
                    roomList.Add(new HashSet<Vector2Int> { point });
            }

            for (var i = 0; i < width - 1; i++)
            {
                var point0 = new Vector2Int(i, height - 1);
                var point1 = new Vector2Int(i + 1, height - 1);
                
                var point0Index = roomList.FindIndex(hashSet => hashSet.Contains(point0));
                var point1Index = roomList.FindIndex(hashSet => hashSet.Contains(point1));
                
                if (point0Index != point1Index)
                {
                    roomList[point0Index].UnionWith(roomList[point1Index]);
                    roomList.RemoveAt(point1Index);
                
                    mapInfo.ConnectRoom(point0, (point1 - point0).ToDirection());
                }
            }
            
            return mapInfo.RefreshDepth(start);
        }
    }
}