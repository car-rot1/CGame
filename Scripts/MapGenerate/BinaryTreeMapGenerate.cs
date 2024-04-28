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

        protected override MapInfo GenerateMap(Vector2Int start, RectInt range)
        {
            var allRoomNum = (range.width + 1) * (range.height + 1);
            var mapInfo = new MapInfo(allRoomNum);
            
            for (var i = range.xMin; i <= range.xMax; i++)
            for (var j = range.yMin; j <= range.yMax; j++)
            {
                mapInfo.AddRoom(new RoomInfo(new Vector2Int(i, j), Color.white));
            }

            for (var i = range.xMin; i <= range.xMax; i++)
            for (var j = range.yMin; j <= range.yMax; j++)
            {
                var point = new Vector2Int(i, j);
                var room = mapInfo.GetRoomInfo(point);
                var directionRandomIndexes = RandomExtension.MultiRange(0, _twoDirections.Length, _twoDirections.Length);
                foreach (var directionRandomIndex in directionRandomIndexes)
                {
                    var targetPoint = point + _twoDirections[directionRandomIndex];
                    if (!range.ContainsIncludeBorder(targetPoint))
                        continue;

                    mapInfo.ConnectRoom(room, (targetPoint - point).ToDirection());
                    break;
                }
            }
            
            return mapInfo.RefreshDepth(start);
        }
    }
}