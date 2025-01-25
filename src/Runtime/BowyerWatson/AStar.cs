using System;
using System.Collections.Generic;
using CGame;

namespace BowyerWatson
{
    public class AStar
    {
        public class Node
        {
            public DirectedEdge Edge { get; }
            public Node Previous { get; private set; }

            public Node(DirectedEdge edge, Node previous)
            {
                Edge = edge;
                Previous = previous;
            }
        }
        
        private readonly Point _start;
        private readonly Point _end;
        private readonly IReadOnlyList<Polygon> _polygons;
        
        private readonly PriorityQueue<Node> _nodePriorityQueue;
        private readonly HashSet<Polygon> _polygonHashSet;
        
        private int Comparison(Node node1, Node node2)
        {
            var sqrMagnitude10 = (node1.Edge.Start - _end).SqrMagnitude();
            var sqrMagnitude11 = (node1.Edge.Next.Start - _end).SqrMagnitude();
            var sqrMagnitude12 = (node1.Edge.Next.Next.Start - _end).SqrMagnitude();
            
            var sqrMagnitude20 = (node2.Edge.Start - _end).SqrMagnitude();
            var sqrMagnitude21 = (node2.Edge.Next.Start - _end).SqrMagnitude();
            var sqrMagnitude22 = (node2.Edge.Next.Next.Start - _end).SqrMagnitude();
            
            return sqrMagnitude10 + sqrMagnitude11 + sqrMagnitude12 > sqrMagnitude20 + sqrMagnitude21 + sqrMagnitude22 ? 1 : -1;
        }
        
        private Polygon _startPolygon;
        private Polygon _endPolygon;

        public AStar(Point start, Point end, IReadOnlyList<Polygon> polygons)
        {
            _start = start;
            _end = end;
            _polygons = polygons;
            _nodePriorityQueue = new PriorityQueue<Node>(Comparison);
            _polygonHashSet = new HashSet<Polygon>();
        }
        
        public void Init()
        {
            _nodePriorityQueue.Clear();
            _polygonHashSet.Clear();
            _startPolygon = null;
            _endPolygon = null;
            foreach (var polygon in _polygons)
            {
                if (polygon.CheckInside(_start))
                {
                    _startPolygon = polygon;
                }
                if (polygon.CheckInside(_end))
                {
                    _endPolygon = polygon;
                }
                if (_startPolygon != null && _endPolygon != null)
                {
                    break;
                }
            }

            if (_startPolygon == null)
            {
                throw new Exception("起始点不在多边形集合中");
            }
            
            if (_endPolygon == null)
            {
                throw new Exception("目标点不在多边形集合中");
            }
        }

        public Node Handle()
        {
            Node lastNode = null;
            
            _nodePriorityQueue.Enqueue(new Node(_startPolygon.FirstEdge, null));
            while (_nodePriorityQueue.Count > 0)
            {
                var node = _nodePriorityQueue.Dequeue();

                var polygon = node.Edge.Polygon;
                if (polygon == _endPolygon)
                {
                    lastNode = node;
                    break;
                }
                
                var edge = polygon.FirstEdge;
                do
                {
                    if (edge.Twin != null && _polygonHashSet.Add(edge.Twin.Polygon))
                    {
                        _nodePriorityQueue.Enqueue(new Node(edge.Twin, node));
                    }
                    edge = edge.Next;
                } while (edge != polygon.FirstEdge);
            }
            return lastNode;
        }
    }
}