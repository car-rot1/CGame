using System;
using UnityEngine;

namespace CGame
{
    public struct Vector2IntBitArray
    {
        public int Count { get; private set; }
        private readonly int[] _bitArray;
        private readonly int _xMin, _xMax, _yMin, _yMax;
        private readonly int _column;

        public Vector2IntBitArray(int xMin, int xMax, int yMin, int yMax)
        {
            Count = 0;
            
            _xMin = xMin;
            _xMax = xMax;
            _yMin = yMin;
            _yMax = yMax;

            _column = xMax - xMin + 1;

            var length = (yMax - yMin + 1) * _column;
            _bitArray = (length & 31) == 0 ? new int[length >> 5] : new int[(length >> 5) + 1];
        }
        
        public Vector2IntBitArray(int row, int column)
        {
            Count = 0;
            
            _xMin = 0;
            _xMax = column - 1;
            _yMin = 0;
            _yMax = row - 1;

            _column = column;

            var length = row * column;
            _bitArray = (length & 31) == 0 ? new int[length >> 5] : new int[(length >> 5) + 1];
        }

        public void Add(Vector2Int vector2Int)
        {
            if (vector2Int.x < _xMin || vector2Int.x > _xMax || vector2Int.y < _yMin || vector2Int.y > _yMax)
                return;

            if (Check(vector2Int))
                return;
            
            var value = (vector2Int.x - _xMin) + (vector2Int.y - _yMin) * _column;
            _bitArray[value >> 5] |= 1 << (value & 31);
            Count++;
        }

        public void UnionWith(Vector2IntBitArray vector2IntBitArray)
        {
            if (_xMin != vector2IntBitArray._xMin || _xMax != vector2IntBitArray._xMax ||
                _yMin != vector2IntBitArray._yMin || _yMax != vector2IntBitArray._yMax)
                throw new Exception("目标与该对象范围不一致");
            Count = 0;
            for (var i = 0; i < vector2IntBitArray._bitArray.Length; i++)
            {
                _bitArray[i] |= vector2IntBitArray._bitArray[i];
                var value = _bitArray[i];
                while (value > 0)
                {
                    value &= (value - 1);
                    Count++;
                }
            }
        }

        public void ExceptWith(Vector2IntBitArray vector2IntBitArray)
        {
            if (_xMin != vector2IntBitArray._xMin || _xMax != vector2IntBitArray._xMax ||
                _yMin != vector2IntBitArray._yMin || _yMax != vector2IntBitArray._yMax)
                throw new Exception("目标与该对象范围不一致");
            Count = 0;
            for (var i = 0; i < vector2IntBitArray._bitArray.Length; i++)
            {
                _bitArray[i] ^= vector2IntBitArray._bitArray[i];
                var value = _bitArray[i];
                while (value > 0)
                {
                    value &= (value - 1);
                    Count++;
                }
            }
        }
        
        public void Remove(Vector2Int vector2Int)
        {
            if (vector2Int.x < _xMin || vector2Int.x > _xMax || vector2Int.y < _yMin || vector2Int.y > _yMax)
                return;
            
            if (!Check(vector2Int))
                return;

            var value = (vector2Int.x - _xMin) + (vector2Int.y - _yMin) * _column;
            _bitArray[value >> 5] ^= 1 << (value & 31);
            Count--;
        }

        public void Clear()
        {
            foreach (var i in _bitArray)
            {
                _bitArray[i] = 0;
            }
            Count = 0;
        }
        
        public bool Check(Vector2Int vector2Int)
        {
            if (vector2Int.x < _xMin || vector2Int.x > _xMax || vector2Int.y < _yMin || vector2Int.y > _yMax)
                return false;

            var value = (vector2Int.x - _xMin) + (vector2Int.y - _yMin) * _column;
            return (_bitArray[value >> 5] & 1 << (value & 31)) != 0;
        }
    }
}