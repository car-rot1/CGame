using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CGame
{
    public class PriorityQueue<T>
    {
        private readonly List<T> _list;
        private readonly Comparison<T> _comparison;

        public PriorityQueue(Comparison<T> comparison, int capacity)
        {
            _list = new List<T>(capacity);
            _comparison = comparison;
        }
        
        public PriorityQueue(Comparison<T> comparison)
        {
            _list = new List<T>();
            _comparison = comparison;
        }

        // public void Enqueue(T value, bool sort = true)
        public void Enqueue(T value)
        {
            var index = _list.Count;
            _list.Add(value);
            // if (!sort)
            //     return;
            
            while (true)
            {
                if (index <= 0)
                    break;
                
                var upIndex = (index - 1) / 2;
                
                if (_comparison.Invoke(_list[index], _list[upIndex]) < 0)
                {
                    (_list[index], _list[upIndex]) = (_list[upIndex], _list[index]);
                }
                else
                    break;
                index = upIndex;
            }
        }

        // public void Sort()
        // {
        //     for (var i = _list.Count / 2; i >= 0; i--)
        //     {
        //         var index = i;
        //         while (true)
        //         {
        //             if (index >= _list.Count - 1)
        //                 break;
        //         
        //             var downIndex0 = index * 2 + 1;
        //             var downIndex1 = index * 2 + 2;
        //
        //             var minIndex = index;
        //             if (downIndex0 < _list.Count && _comparison.Invoke(_list[downIndex0], _list[minIndex]) < 0)
        //                 minIndex = downIndex0;
        //             if (downIndex1 < _list.Count && _comparison.Invoke(_list[downIndex1], _list[minIndex]) < 0)
        //                 minIndex = downIndex1;
        //         
        //             if (minIndex == index)
        //                 break;
        //         
        //             (_list[index], _list[minIndex]) = (_list[minIndex], _list[index]);
        //             index = minIndex;
        //         }
        //     }
        // }

        public T Dequeue()
        {
            var result = _list[0];
            (_list[0], _list[^1]) = (_list[^1], _list[0]);
            _list.RemoveAt(_list.Count - 1);
            
            var index = 0;
            while (true)
            {
                if (index >= _list.Count - 1)
                    break;
                
                var downIndex0 = index * 2 + 1;
                var downIndex1 = index * 2 + 2;

                var minIndex = index;
                if (downIndex0 < _list.Count && _comparison.Invoke(_list[downIndex0], _list[minIndex]) < 0)
                    minIndex = downIndex0;
                if (downIndex1 < _list.Count && _comparison.Invoke(_list[downIndex1], _list[minIndex]) < 0)
                    minIndex = downIndex1;
                
                if (minIndex == index)
                    break;
                
                (_list[index], _list[minIndex]) = (_list[minIndex], _list[index]);
                index = minIndex;
            }
            return result;
        }

        public int Count => _list.Count;
        
        public void Clear() => _list.Clear();
    }
}