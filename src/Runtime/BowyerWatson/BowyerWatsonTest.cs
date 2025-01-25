using System.Collections.Generic;
using CGame;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BowyerWatson
{
    public class BowyerWatsonTest : MonoBehaviour
    {
        public bool drawPoints = true;
        public bool drawBowyerWatson = true;
        public bool drawAstar = true;
        public bool drawFunnelAlgorithm = true;
        
        public int pointCount;
        public Rect pointRect;
        public List<Point> points = new();
        public int index;
        
        private BowyerWatson _bowyerWatson;
        private bool _success = true;
        private Polygon _failedTriangle;
        private Point _failedPoint;
        
        public Point startPoint;
        public Point endPoint;
        private AStar _aStar;
        private AStar.Node _endNode;

        private FunnelAlgorithm _funnelAlgorithm;
        private IReadOnlyList<Point> _path;

        public void CreateRandomPoints()
        {
            points.Clear();
            for (var i = 0; i < pointCount; i++)
            {
                var point = new Point(Random.Range(pointRect.xMin, pointRect.xMax), Random.Range(pointRect.yMin, pointRect.yMax));
                if (points.Contains(point))
                {
                    i--;
                }
                else
                {
                    points.Add(point);
                }
            }
        }
        
        public void BowyerWatsonInit()
        {
            index = 0;
            _bowyerWatson = new BowyerWatson(points);
            _bowyerWatson.Init();
            _success = true;
            _failedTriangle = null;
            _failedPoint = default;

            _path = null;
        }

        public void BowyerWatsonHandle()
        {
            _bowyerWatson.Handle();
        }
        
        public void BowyerWatsonHandleItem()
        {
            _bowyerWatson.HandleItem(index);
        }

        public void BowyerWatsonCheck()
        {
            _success = _bowyerWatson.Check(out _failedTriangle, out _failedPoint);
            Debug.Log(_success ? "Success" : "Failed");
        }
        
        public void AStarInit()
        {
            _aStar = new AStar(startPoint, endPoint, _bowyerWatson.Triangles);
            _aStar.Init();
            _endNode = null;
        }

        public void AStarHandle()
        {
            _endNode = _aStar.Handle();
        }
        
        public void FunnelAlgorithmHandle()
        {
            _funnelAlgorithm = new FunnelAlgorithm(startPoint, endPoint, _endNode);
            _path = _funnelAlgorithm.Handle();
        }

        private void OnDrawGizmosSelected()
        {
            if (drawPoints)
            {
                foreach (var point in points)
                {
                    GizmosExtension.DrawCircle(new Vector2(point.x, point.y), 0.2f);
                }
            }

            if (drawBowyerWatson)
            {
                _bowyerWatson?.Draw();
                if (!_success)
                {
                    Gizmos.color = Color.red;
                    _failedTriangle?.Draw();
                    GizmosExtension.DrawCircle(new Vector2(_failedPoint.x, _failedPoint.y), 0.2f);
                }
            }

            if (drawAstar)
            {
                Gizmos.color = Color.blue;
                GizmosExtension.DrawCircle(new Vector2(startPoint.x, startPoint.y), 0.3f);
                GizmosExtension.DrawCircle(new Vector2(endPoint.x, endPoint.y), 0.3f);
                
                var node = _endNode;
                while (true)
                {
                    if (node == null)
                    {
                        break;
                    }
                    Gizmos.color = Color.red;
                    GizmosExtension.DrawCircle(new Vector2(node.Edge.Start.x, node.Edge.Start.y), 0.2f);
                    GizmosExtension.DrawCircle(new Vector2(node.Edge.Next.Start.x, node.Edge.Next.Start.y), 0.2f);
                    GizmosExtension.DrawCircle(new Vector2(node.Edge.Next.Next.Start.x, node.Edge.Next.Next.Start.y), 0.2f);
                    node.Edge.Polygon.Draw();
                    node = node.Previous;
                }
                node = _endNode;
                while (true)
                {
                    if (node?.Previous == null)
                    {
                        break;
                    }
                    Gizmos.color = Color.green;
                    node.Edge.Draw();
                    node = node.Previous;
                }
            }

            if (drawFunnelAlgorithm)
            {
                if (_path != null)
                {
                    for (var i = 0; i < _path.Count; i++)
                    {
                        var point = _path[i];
                        if (i == 0)
                        {
                            Gizmos.color = Color.cyan;
                        }
                        else if (i == _path.Count - 1)
                        {
                            Gizmos.color = Color.red;
                        }
                        else
                        {
                            Gizmos.color = Color.yellow;
                        }
                        GizmosExtension.DrawCircle(new Vector2(point.x, point.y), 0.2f);
                        if (i < _path.Count - 1)
                        {
                            Gizmos.color = Color.yellow;
                            Gizmos.DrawLine(new Vector2(point.x, point.y), new Vector2(_path[i + 1].x, _path[i + 1].y));
                        }
                    }
                }
            }
        }
    }
}