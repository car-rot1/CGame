using System.Collections.Generic;
using UnityEngine;

namespace CGame
{
    public sealed class KruskalMapGenerate : MapGenerateBase
    {
        public KruskalMapGenerate(int? seed = null) : base(seed)
        {
        }

        protected override MapInfo GenerateMap(Vector2Int start, int width, int height)
        {
            var mapInfo = new MapInfo(width, height, start);
            
            var allRoomNum = width * height;
            for (var i = 0; i < width; i++)
            for (var j = 0; j < height; j++)
            {
                mapInfo.AddRoom(new RoomInfo(new Vector2Int(i, j), RoomType.Other));
            }
            
            var roomList = new List<Vector2IntBitArray>(allRoomNum);
            var wallList = new List<(Vector2Int point0, Vector2Int point1)>((height - 1) * width + (width - 1) * height);
            
            for (var i = 0; i < width; i++)
            for (var j = 0; j < height; j++)
            {
                var point = new Vector2Int(i, j);

                var vector2IntBitArray = new Vector2IntBitArray(height, width);
                vector2IntBitArray.Add(point);
                roomList.Add(vector2IntBitArray);
                
                var point0 = point + Vector2Int.right;
                var point1 = point + Vector2Int.up;
                if (0 <= point0.x && point0.x < width && 0 <= point0.y && point0.y < height)
                    wallList.Add((point, point0));
                if (0 <= point1.x && point1.x < width && 0 <= point1.y && point1.y < height)
                    wallList.Add((point, point1));
            }

            while (wallList.Count > 0)
            {
                var randomWall = wallList.RandomItem(out var randomIndex);
                wallList.RemoveAt(randomIndex);
                var point0Index = roomList.FindIndex(hashSet => hashSet.Check(randomWall.point0));
                var point1Index = roomList.FindIndex(hashSet => hashSet.Check(randomWall.point1));
                if (point0Index == point1Index)
                    continue;

                roomList[point0Index].UnionWith(roomList[point1Index]);
                roomList.RemoveAt(point1Index);

                mapInfo.ConnectRoom(mapInfo.GetRoomInfo(randomWall.point0), (randomWall.point1 - randomWall.point0).ToDirection());
            }

            return mapInfo.RefreshDepth(start);
        }
    }
}