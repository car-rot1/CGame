using UnityEngine;

namespace CGame
{
    public sealed class BinaryTreeMapGenerate : MapGenerateBase
    {
        private readonly Vector2Int[] _twoDirections = new Vector2Int[2];
        
        public BinaryTreeMapGenerate(Direction horizontalDirection, Direction verticalDirection, int? seed = null) : base(seed)
        {
            _twoDirections[0] = horizontalDirection.ToVector();
            if (_twoDirections[0] == Vector2Int.zero)
                _twoDirections[0] = Vector2Int.right;

            _twoDirections[1] = verticalDirection.ToVector();
            if (_twoDirections[1] == Vector2Int.zero)
                _twoDirections[1] = Vector2Int.up;
        }

        protected override MapInfo GenerateMap(Vector2Int start, int width, int height)
        {
            var mapInfo = new MapInfo(width, height, start);
            
            for (var i = 0; i < width; i++)
            for (var j = 0; j < height; j++)
            {
                mapInfo.AddRoom(new RoomInfo(new Vector2Int(i, j), RoomType.Other));
            }

            for (var i = 0; i < width; i++)
            for (var j = 0; j < height; j++)
            {
                var point = new Vector2Int(i, j);
                var room = mapInfo.GetRoomInfo(point);
                var directionRandomIndexes = RandomExtension.MultiRange(0, _twoDirections.Length, _twoDirections.Length);
                foreach (var directionRandomIndex in directionRandomIndexes)
                {
                    var targetPoint = point + _twoDirections[directionRandomIndex];
                    if (targetPoint.x < 0 || targetPoint.x >= width || targetPoint.y < 0 || targetPoint.y >= height)
                        continue;

                    mapInfo.ConnectRoom(room, (targetPoint - point).ToDirection());
                    break;
                }
            }
            
            return mapInfo.RefreshDepth(start);
        }
    }
}