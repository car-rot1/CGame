using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace CGame
{
    [Serializable]
    public sealed class MapInfo
    {
        [JsonRequired] private readonly List<RoomInfo> allRoom;
        private readonly Dictionary<Vector2Int, RoomInfo> _allRoomDic;
        
        public Vector2Int Start { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public MapInfo(int width, int height, Vector2Int start)
        {
            allRoom = new List<RoomInfo>(width * height);
            _allRoomDic = new Dictionary<Vector2Int, RoomInfo>(width * height);
            
            Width = width;
            Height = height;
            Start = start;
        }

        public static string ToJson(MapInfo mapInfo, bool isIndented = false)
        {
            var json = JsonConvert.SerializeObject(mapInfo);

            if (!isIndented)
                return json;
            
            var serializer = new JsonSerializer();
            var tr = new StringReader(json);
            var jtr = new JsonTextReader(tr);
            var obj = serializer.Deserialize(jtr);
            var textWriter = new StringWriter();
            var jsonWriter = new JsonTextWriter(textWriter)
            {
                Formatting = Formatting.Indented,
                Indentation = 4,
                IndentChar = ' '
            };
            serializer.Serialize(jsonWriter, obj);

            return textWriter.ToString();
        }

        public static MapInfo ToMapInfo(string json)
        {
            var result = JsonConvert.DeserializeObject<MapInfo>(json);
            foreach (var roomInfo in result.allRoom)
            {
                result._allRoomDic[roomInfo.position] = roomInfo;
            }
            return result;
        }

        public void AddRoom(RoomInfo room)
        {
            allRoom.Add(room);
            _allRoomDic[room.position] = room;
        }

        public IEnumerable<RoomInfo> GetAllRoom() => allRoom;
        public RoomInfo GetRoomInfo(Vector2Int position) => _allRoomDic.TryGetValue(position, out var roomInfo) ? roomInfo : null;
        
        public bool ConnectRoom(RoomInfo room, Direction connectDirection)
        {
            foreach (Direction direction in connectDirection)
            {
                if (room.connectDirection.HasFlag(direction))
                    continue;
                
                if (!_allRoomDic.TryGetValue(room.position + direction.ToVector(), out var connectRoom))
                    continue;
                
                room.connectDirection |= direction;
                connectRoom.connectDirection |= direction.Reverse();
            }

            return true;
        }
        
        public bool DisConnectRoom(RoomInfo room, Direction disConnectDirection)
        {
            foreach (Direction direction in disConnectDirection)
            {
                if (!room.connectDirection.HasFlag(direction))
                    continue;
                
                if (!_allRoomDic.TryGetValue(room.position + direction.ToVector(), out var disConnectRoom))
                    continue;
                
                room.connectDirection ^= direction;
                disConnectRoom.connectDirection ^= direction.Reverse();
            }

            return true;
        }

        public MapInfo RefreshRooms(Vector2Int? start = null)
        {
            var dic = new Dictionary<Vector2Int, RoomInfo>(_allRoomDic.Count);

            start ??= _allRoomDic.First(pair => pair.Value.depth == 0).Key;
            Start = start.Value;
            
            var queue = new Queue<RoomInfo>();

            var startRoom = _allRoomDic[start.Value];
            startRoom.type = RoomType.Start;
            startRoom.depth = 0;
            startRoom.lastDirection = Direction.None;
            dic.Add(start.Value, startRoom);
            queue.Enqueue(startRoom);

            var points = new List<Vector2Int>(4);
            while (queue.Count > 0)
            {
                var nowQueueLenght = queue.Count;
                for (var i = 0; i < nowQueueLenght; i++)
                {
                    var room = queue.Dequeue();
                    points.Clear();
                    foreach (Direction direction in room.connectDirection)
                        points.Add(room.position + direction.ToVector());
                    foreach (var point in points)
                    {
                        if (dic.ContainsKey(point))
                            continue;
                        
                        var nextRoom = _allRoomDic[point];
                        nextRoom.type = RoomType.Other;
                        nextRoom.depth = room.depth + 1;
                        nextRoom.lastDirection = (room.position - point).ToDirection();
                        dic.Add(point, nextRoom);
                        queue.Enqueue(nextRoom);
                    }
                }
            }
            
            _allRoomDic.Clear();
            foreach (var keyValuePair in dic)
                _allRoomDic.Add(keyValuePair.Key, keyValuePair.Value);

            return this;
        }

        public MapInfo RefreshDepth(Vector2Int? start = null)
        {
            start ??= _allRoomDic.First(pair => pair.Value.depth == 0).Key;
            Start = start.Value;
            
            var currentRoomPositions = new Vector2IntBitArray(Height, Width);
            var queue = new Queue<RoomInfo>();

            var startRoom = _allRoomDic[start.Value];
            startRoom.type = RoomType.Start;
            startRoom.depth = 0;
            startRoom.lastDirection = Direction.None;
            currentRoomPositions.Add(start.Value);
            queue.Enqueue(startRoom);

            var points = new List<Vector2Int>(4);
            while (queue.Count > 0)
            {
                var nowQueueLenght = queue.Count;
                for (var i = 0; i < nowQueueLenght; i++)
                {
                    var room = queue.Dequeue();
                    points.Clear();
                    foreach (Direction direction in room.connectDirection)
                        points.Add(room.position + direction.ToVector());
                    foreach (var point in points)
                    {
                        if (currentRoomPositions.Check(point))
                            continue;
                        
                        var nextRoom = _allRoomDic[point];
                        nextRoom.type = RoomType.Other;
                        nextRoom.depth = room.depth + 1;
                        nextRoom.lastDirection = (room.position - point).ToDirection();
                        currentRoomPositions.Add(point);
                        queue.Enqueue(nextRoom);
                    }
                }
            }

            return this;
        }

        public MapInfo SplitMainRoom(int mainRoomNum, List<RoomInfo> mainRooms, List<RoomInfo> otherRooms)
        {
            mainRooms.Clear();
            otherRooms.Clear();
            
            mainRoomNum = Mathf.Min(_allRoomDic.Values.Max(roomInfo => roomInfo.depth), mainRoomNum - 1);
            
            otherRooms.AddRange(_allRoomDic.Values);
            var indexList = new List<int>(otherRooms.Count);
            for (var i = 0; i < otherRooms.Count; i++)
            {
                if (otherRooms[i].depth == mainRoomNum)
                    indexList.Add(i);
            }

            var randomIndex = indexList.RandomItem();
            mainRooms.Add(otherRooms[randomIndex]);
            otherRooms.RemoveAt(randomIndex);
            
            var room = mainRooms[0];
            while (room.lastDirection is not Direction.None)
            {
                var lastPosition = room.position + room.lastDirection.ToVector();
                room = _allRoomDic[lastPosition];
                mainRooms.Add(room);
                otherRooms.Remove(room);
            }
            mainRooms.Reverse();

            return this;
        }
        
        public MapInfo SplitRoom(List<List<RoomInfo>> result)
        {
            result.Clear();
            
            var list = _allRoomDic.Values.ToList();
            list.Sort((roomInfo0, roomInfo1) => roomInfo0.depth - roomInfo1.depth);
            var rooms = new LinkedList<RoomInfo>(list);

            var index = 0;
            while (rooms.Count > 0)
            {
                var room = rooms.Last.Value;
                rooms.RemoveLast();
                result.Add(new List<RoomInfo> { room });
                while (true)
                {
                    var lastPosition = room.position + room.lastDirection.ToVector();
                    room = _allRoomDic[lastPosition];
                    if (rooms.Remove(room))
                        result[index].Add(room);
                    else
                        break;
                }
                result[index].Reverse();
                index++;
            }

            return this;
            
            /* 一个地图生成规则
            s = _mapInfo.SplitRoom();

            //确定主房间的个数
            _mainRooms.Clear();
            foreach (var roomInfo in s[0].TakeWhile(roomInfo => roomInfo.depth < mainRoomNum))
            {
                _mainRooms.Add(roomInfo);
            }
            s.RemoveAt(0);
        
            //删除在起点或终点上的支路
            var hashSet = new HashSet<Vector2Int>();
            for (var i = 0; i < s.Count; i++)
            {
                var roomInfos = s[i];
            
                if (roomInfos[0].depth <= 1 || roomInfos[0].depth >= mainRoomNum - 1)
                {
                    foreach (var roomInfo in roomInfos)
                        hashSet.Add(roomInfo.position);
                    s.RemoveAt(i);
                    i--;
                }
            }
        
            //删除因前面删除支路而不联通的支路
            var b = true;
            while (b)
            {
                b = false;
                for (var i = 0; i < s.Count; i++)
                {
                    var roomInfos = s[i];
                    if (hashSet.Contains(roomInfos[0].position + roomInfos[0].lastDirection.ToVector()))
                    {
                        foreach (var roomInfo in roomInfos)
                            hashSet.Add(roomInfo.position);
                        s.RemoveAt(i);
                        i--;
                        b = true;
                    }
                }
            }
            */
        }
    }
}