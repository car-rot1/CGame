using System.Collections.Generic;
using UnityEngine;

namespace CGame
{
    public sealed class DFSMapGenerate : MapGenerateBase
    {
        public DFSMapGenerate(int? seed = null) : base(seed)
        {
        }

        protected override MapInfo GenerateMap(Vector2Int start, int width, int height)
        {
            var mapInfo = new MapInfo(width, height, start);

            var allRoomNum = width * height;
            var currentRoomPositions = new Vector2IntBitArray(height, width);
            var stack = new Stack<Vector2Int>(allRoomNum);

            currentRoomPositions.Add(start);
            stack.Push(start);

            var depth = 0;
            mapInfo.AddRoom(new RoomInfo(start, RoomType.Start));
            
            while (currentRoomPositions.Count < allRoomNum)
            {
                var directionRandomIndexes = RandomExtension.MultiRange(0, directions.Length, directions.Length);
                int i;
                for (i = 0; i < directionRandomIndexes.Length; i++)
                {
                    var lastPoint = stack.Peek();
                    var targetPoint = lastPoint + directions[directionRandomIndexes[i]];
                    
                    if (targetPoint.x < 0 || targetPoint.x >= width || targetPoint.y < 0 || targetPoint.y >= height || currentRoomPositions.Check(targetPoint))
                        continue;
                    
                    currentRoomPositions.Add(targetPoint);
                    stack.Push(targetPoint);
                    
                    depth++;
                    
                    var lastRoom = mapInfo.GetRoomInfo(lastPoint);
                    var room = new RoomInfo(targetPoint, RoomType.Other, depth, (lastPoint - targetPoint).ToDirection());

                    lastRoom.connectDirection |= (targetPoint - lastPoint).ToDirection();
                    room.connectDirection |= (lastPoint - targetPoint).ToDirection();
                    mapInfo.AddRoom(room);
                    
                    break;
                }

                if (i >= directionRandomIndexes.Length)
                {
                    stack.Pop();
                    depth--;
                }
            }

            return mapInfo;
        }
    }
}
