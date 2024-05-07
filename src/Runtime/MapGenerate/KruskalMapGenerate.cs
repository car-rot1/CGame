using System.Collections.Generic;
using UnityEngine;

namespace CGame
{
    public sealed class KruskalMapGenerate : MapGenerateBase
    {
        public KruskalMapGenerate(int? seed = null) : base(seed)
        {
        }

        protected override MapInfo GenerateMap(Vector2Int start, RectInt range)
        {
            var allRoomNum = (range.width + 1) * (range.height + 1);
            var mapInfo = new MapInfo(allRoomNum, start);
            
            for (var i = range.xMin; i <= range.xMax; i++)
            for (var j = range.yMin; j <= range.yMax; j++)
            {
                mapInfo.AddRoom(new RoomInfo(new Vector2Int(i, j), Color.white));
            }
            
            var roomList = new List<HashSet<Vector2Int>>((range.width + 1) * (range.height + 1));
            var wallList = new List<(Vector2Int point0, Vector2Int point1)>(range.height * (range.width + 1) + range.width * (range.height + 1));
            
            for (var i = range.xMin; i <= range.xMax; i++)
            for (var j = range.yMin; j <= range.yMax; j++)
            {
                var point = new Vector2Int(i, j);

                roomList.Add(new HashSet<Vector2Int> { point });
                
                var point0 = point + Vector2Int.right;
                var point1 = point + Vector2Int.up;
                if (range.ContainsIncludeBorder(point0))
                    wallList.Add((point, point0));
                if (range.ContainsIncludeBorder(point1))
                    wallList.Add((point, point1));
            }

            while (wallList.Count > 0)
            {
                var randomWall = wallList.RandomItem(out var randomIndex);
                wallList.RemoveAt(randomIndex);
                var point0Index = roomList.FindIndex(hashSet => hashSet.Contains(randomWall.point0));
                var point1Index = roomList.FindIndex(hashSet => hashSet.Contains(randomWall.point1));
                if (point0Index == point1Index)
                    continue;

                roomList[point0Index].UnionWith(roomList[point1Index]);
                roomList.RemoveAt(point1Index);

                mapInfo.ConnectRoom(randomWall.point0, (randomWall.point1 - randomWall.point0).ToDirection());
            }

            return mapInfo.RefreshDepth(start);
        }
    }
}