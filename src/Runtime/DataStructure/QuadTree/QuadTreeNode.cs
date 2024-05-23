using System.Collections.Generic;
using System.Linq;
using CGame;
using UnityEngine;

public abstract class QuadTreeNodeBase<TNode, TPoint> where TNode : QuadTreeNodeBase<TNode, TPoint>
{
    public Rect Rect { get; }

    public TNode[] ChildrenNode { get; private set; }

    private readonly int _maxPointNum;
    private readonly int _maxLevel;
    private readonly int _currentLevel;
    
    private readonly TNode _rootNode;
    private TNode _parentNode;

    private readonly List<TPoint> _points;
    protected abstract Vector2 GetPointPosition(TPoint point);

    protected QuadTreeNodeBase(Rect rect, int maxPointNum, int maxLevel, int currentLevel = 0, TNode rootNode = null, TNode parentNode = null)
    {
        Rect = rect;
        _maxPointNum = maxPointNum;
        _maxLevel = maxLevel;
        _currentLevel = currentLevel;
        _rootNode = rootNode ?? (TNode)this;
        _parentNode = parentNode;
        _points = new List<TPoint>();
    }

    public void AddPoint(TPoint point)
    {
        if (ChildrenNode == null)
        {
            _points.Add(point);
            if (SplitCheck((TNode)this))
                Split();
        }
        else
        {
            var index = GetQuadrantIndex(GetPointPosition(point));
            ChildrenNode[index].AddPoint(point);
        }
    }

    private static bool SplitCheck(TNode node) => node._points.Count > node._maxPointNum && node._currentLevel < node._maxLevel;

    public void RemovePoint(TPoint point)
    {
        if (ChildrenNode == null)
        {
            _points.Remove(point);
            var parent = _parentNode;
            while (parent != null)
            {
                if (!UniteCheck(parent))
                    break;
                parent.Unite();
                parent = parent._parentNode;
            }
        }
        else
        {
            var index = GetQuadrantIndex(GetPointPosition(point));
            ChildrenNode[index].RemovePoint(point);
        }
    }

    private static bool UniteCheck(TNode node)
    {
        var pointNum = 0;
        foreach (var childNode in node.ChildrenNode)
        {
            if (childNode.ChildrenNode != null)
                return false;
            pointNum += childNode._points.Count;
        }
        return pointNum <= node._maxPointNum;
    }

    public void UpdatePoint()
    {
        if (ChildrenNode == null)
        {
            if (_parentNode == null)
                return;
            for (var i = _points.Count - 1; i >= 0; i--)
            {
                var point = _points[i];
                var rect = Rect;
                //松散四叉树（即扩大判断是否删除点的范围）
                var center = rect.center;
                rect.width += rect.width;
                rect.height += rect.height;
                rect.center = center;
                if (rect.Contains(GetPointPosition(point)))
                    continue;
                RemovePoint(point);
                _rootNode.AddPoint(point);
            }

            return;
        }
        foreach (var childNode in ChildrenNode)
        {
            childNode?.UpdatePoint();
        }
    }
    
    private void Split()
    {
        var childrenRect = Rect.QuadSplitForNum(2, 2);

        ChildrenNode = new TNode[4];
        
        for (var i = 0; i < ChildrenNode.GetLength(0); i++)
        for (var j = 0; j < ChildrenNode.GetLength(1); j++)
            ChildrenNode[i * 2 + j] = CreateChildNode(childrenRect[i, j], _rootNode, (TNode)this);
        
        for (var i = _points.Count - 1; i >= 0; i--)
        {
            var point = _points[i];
            _points.RemoveAt(i);
            var index = GetQuadrantIndex(GetPointPosition(point));
            ChildrenNode[index].AddPoint(point);
        }
    }

    private void Unite()
    {
        for (var i = 0; i < ChildrenNode.Length; i++)
        {
            _points.AddRange(ChildrenNode[i]._points);
            ChildrenNode[i]._parentNode = null;
            ChildrenNode[i] = null;
        }

        ChildrenNode = null;
    }

    protected abstract TNode CreateChildNode(Rect rect, int maxPointNum, int maxLevel, int currentLevel = 0, TNode rootNode = null, TNode parentNode = null);

    private TNode CreateChildNode(Rect childRect, TNode root, TNode parent) => CreateChildNode(childRect, _maxPointNum, _maxLevel, _currentLevel + 1, root, parent);

    /*
     * 与 Rect.Contains(position) 对应
     * point.x >= this.xMin && point.x < this.xMax && point.y >= this.yMin && point.y < this.yMax;
     * 只包含最小不包含最大，因此选择象限时优先选择右上角的象限（这样可以使该点落在最小值而非最大值里）
     */
    private int GetQuadrantIndex(Vector2 position) => position.y >= Rect.center.y ? position.x >= Rect.center.x ? 1 : 0 : position.x >= Rect.center.x ? 3 : 2;

    public List<TPoint> GetAllPoint(Vector2 position, float radius)
    {
        UpdatePoint();
        var list = new List<TPoint>();
        GetAllPoint(position, radius, list);
        return list;
    }

    public int GetAllPointNonAlloc(Vector2 position, float radius, TPoint[] array)
    {
        UpdatePoint();
        return GetAllPointNonAlloc(position, radius, array, 0);
    }

    private void GetAllPoint(Vector2 position, float radius, List<TPoint> allPointResult)
    {
        if (ChildrenNode == null)
        {
            allPointResult.AddRange(_points);
            return;
        }

        foreach (var childNode in ChildrenNode)
        {
            if (childNode.CheckCurrentRect(position, radius))
                childNode.GetAllPoint(position, radius, allPointResult);
        }
    }
    
    private int GetAllPointNonAlloc(Vector2 position, float radius, TPoint[] allPointResult, int length)
    {
        if (ChildrenNode == null)
        {
            for (var i = 0; i < _points.Count; i++)
                allPointResult[length + i] = _points[i];
            return length + _points.Count;
        }

        foreach (var childNode in ChildrenNode)
        {
            if (childNode.CheckCurrentRect(position, radius))
                length = childNode.GetAllPointNonAlloc(position, radius, allPointResult, length);
        }

        return length;
    }

    private bool CheckCurrentRect(Vector2 position, float radius)
    {
        if (Rect.Contains(position))
            return true;
        var center = Rect.center;
        var xDistance = Mathf.Abs(position.x - center.x) - Rect.width / 2f;
        var yDistance = Mathf.Abs(position.y - center.y) - Rect.height / 2f;
        if (xDistance <= 0)
            return yDistance <= radius;
        if (yDistance <= 0)
            return xDistance <= radius;

        return xDistance * xDistance + yDistance * yDistance <= radius * radius;
    }
}

public sealed class QuadTreeNodeTransform : QuadTreeNodeBase<QuadTreeNodeTransform, Transform>
{
    public QuadTreeNodeTransform(Rect rect, int maxPointNum, int maxLevel, int currentLevel = 0,
        QuadTreeNodeTransform rootNode = null, QuadTreeNodeTransform parentNode = null) : base(rect, maxPointNum,
        maxLevel, currentLevel, rootNode, parentNode) { }

    protected override Vector2 GetPointPosition(Transform point) => point.position;

    protected override QuadTreeNodeTransform CreateChildNode(Rect rect, int maxPointNum, int maxLevel,
        int currentLevel = 0, QuadTreeNodeTransform rootNode = null, QuadTreeNodeTransform parentNode = null) =>
        new(rect, maxPointNum, maxLevel, currentLevel, rootNode, parentNode);
}

public interface IPoint
{
    Vector2 Position { get; }
}

public sealed class QuadTreeNodeIPoint : QuadTreeNodeBase<QuadTreeNodeIPoint, IPoint>
{
    public QuadTreeNodeIPoint(Rect rect, int maxPointNum, int maxLevel, int currentLevel = 0,
        QuadTreeNodeIPoint rootNode = null, QuadTreeNodeIPoint parentNode = null) : base(rect, maxPointNum,
        maxLevel, currentLevel, rootNode, parentNode) { }
    
    protected override Vector2 GetPointPosition(IPoint point) => point.Position;
    
    protected override QuadTreeNodeIPoint CreateChildNode(Rect rect, int maxPointNum, int maxLevel,
        int currentLevel = 0, QuadTreeNodeIPoint rootNode = null, QuadTreeNodeIPoint parentNode = null) =>
        new(rect, maxPointNum, maxLevel, currentLevel, rootNode, parentNode);
}