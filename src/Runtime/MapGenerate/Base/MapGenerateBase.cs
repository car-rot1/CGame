using UnityEngine;
using Random = UnityEngine.Random;

namespace CGame
{
    /*  调试代码
    private MapInfo _mapInfo;
    public int depth;
    public int seed;
    
    [Button]
    private void AddDepth() => depth++;
    
    [Button]
    private void DelDepth() => depth--;
    
    [Button]
    private void TTTTT()
    {
        bool a = transform;
        var mapG = new EllerMapGenerate(new Vector2Int(1, 4), new Vector2Int(1, 4));
        _mapInfo = mapG.Generate(5, 5);
        seed = mapG.Seed;
    }
    
    private void OnDrawGizmos()
    {
        if (_mapInfo == null)
            return;
        foreach (var roomInfo in _mapInfo.AllRoom)
        {
            var position = roomInfo.position + (Vector2)transform.position;
            Gizmos.color = roomInfo.depth == depth ? Color.black : Color.white; 
            GizmosExtension.DrawCircle(position, 0.2f);
            Gizmos.color = Color.white;
            foreach (Direction direction in roomInfo.connectDirection)
            {
                Gizmos.DrawLine(position, roomInfo.position + direction.ToVector() + (Vector2)transform.position);
            }
        }
    }
    */
    
    public abstract class MapGenerateBase
    {
        protected readonly Vector2Int[] directions =
        {
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left,
        };

        public readonly int Seed;
        private int _seed;

        protected MapGenerateBase(int? seed)
        {
            Seed = seed ?? new System.Random().Next(int.MinValue, int.MaxValue);
            _seed = Seed;
        }

        public MapInfo Generate(int width, int height)
        {
            var defaultState = Random.state;
            Random.InitState(_seed);

            var start = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
            var result = GenerateMap(start, width, height);
            
            Random.state = defaultState;
            _seed++;
            return result;
        }
        
        public MapInfo Generate(Vector2Int start, int width, int height)
        {
            var defaultState = Random.state;
            Random.InitState(_seed);

            var result = GenerateMap(start, width, height);
            
            Random.state = defaultState;
            _seed++;
            return result;
        }
        
        protected abstract MapInfo GenerateMap(Vector2Int start, int width, int height);
    }
}