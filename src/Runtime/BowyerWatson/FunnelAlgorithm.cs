using System.Collections.Generic;

namespace BowyerWatson
{
    public class FunnelAlgorithm
    {
        public class Node
        {
            public Point StartPoint { get; }
            public Point EndPoint { get; }
            public Node Next { get; }

            public Node(Point startPoint, Point endPoint, Node next)
            {
                StartPoint = startPoint;
                EndPoint = endPoint;
                Next = next;
            }
        }
        
        private readonly Point _start;
        private readonly Point _end;
        private readonly AStar.Node _aStarNode;
        private Node _node;

        private readonly List<Point> _path = new();
        private Point _currentLeftPoint;
        private Point _currentRightPoint;

        private readonly Dictionary<Point, Node> _pointNodeDic = new();
        
        public FunnelAlgorithm(Point start, Point end, AStar.Node aStarNode)
        {
            _start = start;
            _end = end;
            _aStarNode = aStarNode;
        }

        public IReadOnlyList<Point> Handle()
        {
            _pointNodeDic.Clear();
            
            Node lastNode = null;
            var currentAStarNode = _aStarNode;
            AStar.Node lastAStarNode = null;
            while (currentAStarNode != null)
            {
                if (lastAStarNode == null)
                {
                    lastNode = new Node(_end, _end, null);
                }
                else
                {
                    lastNode = new Node(lastAStarNode.Edge.End, lastAStarNode.Edge.Start, lastNode);
                    _pointNodeDic[lastAStarNode.Edge.Twin.Next.End] = lastNode;
                }
                lastAStarNode = currentAStarNode;
                currentAStarNode = currentAStarNode.Previous;
            }
            _node = lastNode;
            
            _path.Clear();
            _path.Add(_start);

            if (_node == null)
            {
                return _path;
            }
            
            _currentLeftPoint = _node.EndPoint;
            _currentRightPoint = _node.StartPoint;
            
            var lastLeftPoint = _currentLeftPoint;
            var lastRightPoint = _currentRightPoint;
            
            var currentNode = _node.Next;
            while (currentNode != null)
            {
                var tempNode = currentNode;
                currentNode = currentNode.Next;
                
                var point = _path[^1];
                if (tempNode.StartPoint == tempNode.EndPoint)
                {
                    _path.Add(_end);
                    break;
                }

                if (tempNode.EndPoint != lastLeftPoint)
                {
                    lastLeftPoint = tempNode.EndPoint;

                    var leftVector = _currentLeftPoint - point;
                    var rightVector = _currentRightPoint - point;
                    var tempLeftVector = tempNode.EndPoint - point;

                    var crossTempToLeftVector = tempLeftVector.x * leftVector.y - tempLeftVector.y * leftVector.x;
                    var crossTempToRightVector = tempLeftVector.x * rightVector.y - tempLeftVector.y * rightVector.x;

                    if (crossTempToLeftVector >= 0)
                    {
                        if (crossTempToRightVector > 0)
                        {
                            _path.Add(_currentRightPoint);
                            if (_pointNodeDic.TryGetValue(_currentRightPoint, out currentNode))
                            {
                                _currentLeftPoint = currentNode.EndPoint;
                                _currentRightPoint = currentNode.StartPoint;
                                lastLeftPoint = _currentLeftPoint;
                                lastRightPoint = _currentRightPoint;
                            }
                            else
                            {
                                _path.Add(_end);
                                break;
                            }
                        }
                        else if (crossTempToRightVector < 0)
                        {
                            _currentLeftPoint = tempNode.EndPoint;
                        }
                    }
                }
                else if (tempNode.StartPoint != lastRightPoint)
                {
                    lastRightPoint = tempNode.StartPoint;

                    var leftVector = _currentLeftPoint - point;
                    var rightVector = _currentRightPoint - point;
                    var tempRightVector = tempNode.StartPoint - point;

                    var crossTempToLeftVector = tempRightVector.x * leftVector.y - tempRightVector.y * leftVector.x;
                    var crossTempToRightVector = tempRightVector.x * rightVector.y - tempRightVector.y * rightVector.x;

                    if (crossTempToRightVector <= 0)
                    {
                        if (crossTempToLeftVector < 0)
                        {
                            _path.Add(_currentLeftPoint);
                            if (_pointNodeDic.TryGetValue(_currentLeftPoint, out currentNode))
                            {
                                _currentLeftPoint = currentNode.EndPoint;
                                _currentRightPoint = currentNode.StartPoint;
                                lastLeftPoint = _currentLeftPoint;
                                lastRightPoint = _currentRightPoint;
                            }
                            else
                            {
                                _path.Add(_end);
                                break;
                            }
                        }
                        else if (crossTempToLeftVector > 0)
                        {
                            _currentRightPoint = tempNode.StartPoint;
                        }
                    }
                }
            }

            return _path;
        }
    }
}