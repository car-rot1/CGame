using System.Collections.Generic;
using UnityEngine;

namespace CGame
{
    public sealed class HuntAndKillMapGenerate : MapGenerateBase
    {
        public HuntAndKillMapGenerate(int? seed = null) : base(seed)
        {
        }

        protected override MapInfo GenerateMap(Vector2Int start, int width, int height)
        {
            var mapInfo = new MapInfo(width, height, start);
            
            var allRoomNum = width * height;
            var currentRoomPositions = new HashSet<Vector2Int>(allRoomNum);

            var lastPoint = start;
            currentRoomPositions.Add(lastPoint);
            mapInfo.AddRoom(new RoomInfo(lastPoint, RoomType.Start));
            
            while (currentRoomPositions.Count < allRoomNum)
            {
                var directionRandomIndexes = RandomExtension.MultiRange(0, directions.Length, directions.Length);
                int i;
                for (i = 0; i < directionRandomIndexes.Length; i++)
                {
                    var targetPoint = lastPoint + directions[directionRandomIndexes[i]];
                    
                    if (targetPoint.x < 0 || targetPoint.x >= width || targetPoint.y < 0 || targetPoint.y >= height || currentRoomPositions.Contains(targetPoint))
                        continue;
                    currentRoomPositions.Add(targetPoint);

                    var lastRoom = mapInfo.GetRoomInfo(lastPoint);
                    var room = new RoomInfo(targetPoint, RoomType.Other, lastRoom.depth + 1, (lastPoint - targetPoint).ToDirection());
                    
                    lastRoom.connectDirection |= (targetPoint - lastPoint).ToDirection();
                    room.connectDirection |= (lastPoint - targetPoint).ToDirection();
                    mapInfo.AddRoom(room);
                    
                    lastPoint = targetPoint;
                    break;
                }

                if (i >= directionRandomIndexes.Length)
                {
                    var currentPointList = new List<Vector2Int>(currentRoomPositions);
                    var pointRandomIndexes = RandomExtension.MultiRange(0, currentPointList.Count, currentPointList.Count);
                    foreach (var pointRandomIndex in pointRandomIndexes)
                    {
                        var point = currentPointList[pointRandomIndex];
                        directionRandomIndexes = RandomExtension.MultiRange(0, directions.Length, directions.Length);
                        for (i = 0; i < directionRandomIndexes.Length; i++)
                        {
                            var targetPoint = point + directions[directionRandomIndexes[i]];
                    
                            if (targetPoint.x < 0 || targetPoint.x >= width || targetPoint.y < 0 || targetPoint.y >= height || currentRoomPositions.Contains(targetPoint))
                                continue;
                            currentRoomPositions.Add(targetPoint);

                            var lastRoom = mapInfo.GetRoomInfo(point);
                            var room = new RoomInfo(targetPoint, RoomType.Other, lastRoom.depth + 1, (point - targetPoint).ToDirection());
                            
                            lastRoom.connectDirection |= (targetPoint - point).ToDirection();
                            room.connectDirection |= (point - targetPoint).ToDirection();
                            mapInfo.AddRoom(room);
                            
                            lastPoint = targetPoint;
                            break;
                        }
                        if (i < directionRandomIndexes.Length)
                            break;
                    }
                }
            }

            return mapInfo;
        }
    }
}