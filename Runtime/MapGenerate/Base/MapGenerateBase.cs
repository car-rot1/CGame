using UnityEngine;
using Random = UnityEngine.Random;

namespace CGame
{
    public abstract class MapGenerateBase
    {
        protected readonly Vector2Int[] directions =
        {
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left,
        };

        private int _seed;

        protected MapGenerateBase(int? seed)
        {
            _seed = seed ?? new System.Random().Next(int.MinValue, int.MaxValue);
        }

        public MapInfo Generate(RectInt range)
        {
            var defaultState = Random.state;
            Random.InitState(_seed);

            var start = new Vector2Int(Random.Range(range.xMin, range.xMax + 1), Random.Range(range.yMin, range.yMax + 1));
            var result = GenerateMap(start, range);
            
            Random.state = defaultState;
            _seed++;
            return result;
        }
        
        public MapInfo Generate(Vector2Int start, RectInt range)
        {
            var defaultState = Random.state;
            Random.InitState(_seed);

            var result = GenerateMap(start, range);
            
            Random.state = defaultState;
            _seed++;
            return result;
        }
        
        protected abstract MapInfo GenerateMap(Vector2Int start, RectInt range);
    }
}