using System;

namespace CGame
{
    public class RedPointNode
    {
        private int _pointNum;
        public int PointNum
        {
            get => _pointNum;
            private set
            {
                if (value == _pointNum)
                    return;
                onPointNumChange?.Invoke(_pointNum, value);
                _pointNum = value;
            }
        }
        public event Action<int, int> onPointNumChange;
        
        public RedPointNode Parent { get; }
        public bool IsLeaf { get; }

        public RedPointNode(RedPointNode parent = null, bool isLeaf = false)
        {
            PointNum = 0;
            Parent = parent;
            IsLeaf = isLeaf;
        }

        public void AddPoint(int point)
        {
            if (!IsLeaf)
                return;
            
            PointNum += point;
            var p = Parent;
            while (p != null)
            {
                p.PointNum += point;
                p = p.Parent;
            }
        }
        
        public void RemovePoint(int point)
        {
            if (PointNum <= 0 || !IsLeaf)
                return;
            
            PointNum -= point;
            var p = Parent;
            while (p != null)
            {
                p.PointNum -= point;
                p = p.Parent;
            }
        }
    }
}