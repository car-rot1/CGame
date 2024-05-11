﻿using System.Collections.Generic;
using UnityEngine;

namespace CGame
{
    public sealed class PrimMapGenerate : MapGenerateBase
    {
        public PrimMapGenerate(int? seed = null) : base(seed)
        {
        }

        protected override MapInfo GenerateMap(Vector2Int start, int width, int height)
        {
            var allRoomNum = width * height;
            var mapInfo = new MapInfo(allRoomNum, start);

            var currentRoomPositions = new HashSet<Vector2Int>(allRoomNum) { start };
            var currentValidRoom = new List<(RoomInfo lastRoomInfo, Vector2Int currentPoint)>(allRoomNum * 3) { (null, start) };
            
            while (currentValidRoom.Count > 0)
            {
                var randomIndex = Random.Range(0, currentValidRoom.Count);
                var (lastRoom, currentPoint) = currentValidRoom[randomIndex];
                currentValidRoom.RemoveAt(randomIndex);

                RoomInfo room;
                if (lastRoom != null)
                {
                    room = new RoomInfo(currentPoint, Color.white, lastRoom.depth + 1, (lastRoom.position - currentPoint).ToDirection());

                    lastRoom.connectDirection |= (currentPoint - lastRoom.position).ToDirection();
                    room.connectDirection |= (lastRoom.position - currentPoint).ToDirection();
                }
                else
                {
                    room = new RoomInfo(currentPoint, Color.blue);
                }
                    
                mapInfo.AddRoom(room);
                foreach (var direction in directions)
                {
                    var targetPoint = currentPoint + direction;
                    if (targetPoint.x < 0 || targetPoint.x >= width || targetPoint.y < 0 || targetPoint.y >= height || currentRoomPositions.Contains(targetPoint))
                        continue;
                    currentRoomPositions.Add(targetPoint);
                    currentValidRoom.Add((room, targetPoint));
                }
            }

            return mapInfo;
        }
    }
}