using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CGame
{
    public sealed class MapInfo
    {
        private readonly Dictionary<Vector2Int, RoomInfo> _allRoomDic;

        public MapInfo(int allRoomNum)
        {
            _allRoomDic = new Dictionary<Vector2Int, RoomInfo>(allRoomNum);
        }

        public void AddRoom(RoomInfo room) => _allRoomDic[room.position] = room;
        
        public RoomInfo GetRoomInfo(Vector2Int position) => _allRoomDic[position];

        public IEnumerable<RoomInfo> AllRoom => _allRoomDic.Values;
        
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
        
        public bool ConnectRoom(Vector2Int roomPosition, Direction connectDirection)
        {
            return _allRoomDic.TryGetValue(roomPosition, out var room) && ConnectRoom(room, connectDirection);
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
        
        public bool DisConnectRoom(Vector2Int roomPosition, Direction disConnectDirection)
        {
            return _allRoomDic.TryGetValue(roomPosition, out var room) && DisConnectRoom(room, disConnectDirection);
        }

        public MapInfo RefreshRooms(Vector2Int? start = null)
        {
            var dic = new Dictionary<Vector2Int, RoomInfo>(_allRoomDic.Count);

            start ??= _allRoomDic.First(pair => pair.Value.depth == 0).Key;
            
            var queue = new Queue<RoomInfo>();

            var startRoom = _allRoomDic[start.Value];
            startRoom.color = Color.blue;
            startRoom.depth = 0;
            startRoom.lastDirection = Direction.None;
            dic.Add(start.Value, startRoom);
            queue.Enqueue(startRoom);
            
            while (queue.Count > 0)
            {
                var nowQueueLenght = queue.Count;
                for (var i = 0; i < nowQueueLenght; i++)
                {
                    var room = queue.Dequeue();
                    var points = new List<Vector2Int>(4);
                    foreach (Direction direction in room.connectDirection)
                        points.Add(room.position + direction.ToVector());
                    foreach (var point in points)
                    {
                        if (dic.ContainsKey(point))
                            continue;
                        
                        var nextRoom = _allRoomDic[point];
                        nextRoom.color = Color.white;
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
            
            var currentRoomPositions = new HashSet<Vector2Int>(_allRoomDic.Count);
            var queue = new Queue<RoomInfo>();

            var startRoom = _allRoomDic[start.Value];
            startRoom.color = Color.blue;
            startRoom.depth = 0;
            startRoom.lastDirection = Direction.None;
            currentRoomPositions.Add(start.Value);
            queue.Enqueue(startRoom);
            
            while (queue.Count > 0)
            {
                var nowQueueLenght = queue.Count;
                for (var i = 0; i < nowQueueLenght; i++)
                {
                    var room = queue.Dequeue();
                    var points = new List<Vector2Int>(4);
                    foreach (Direction direction in room.connectDirection)
                        points.Add(room.position + direction.ToVector());
                    foreach (var point in points)
                    {
                        if (currentRoomPositions.Contains(point))
                            continue;
                        
                        var nextRoom = _allRoomDic[point];
                        nextRoom.color = Color.white;
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