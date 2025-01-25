using System;
using System.Collections.Generic;
using UnityEngine;

namespace BowyerWatson
{
    public class Polygon
    {
        public DirectedEdge FirstEdge { get; private set; }
        public int EdgeCount { get; private set; }

        #region SetValue

        public Polygon SetFirstEdge(DirectedEdge edge, int edgeCount)
        {
            FirstEdge = edge;
            EdgeCount = edgeCount;
            return this;
        }

        #endregion

        public static Polygon CreateSuperTriangle(IReadOnlyList<Point> points)
        {
            var leftBottomPoint = points[0];
            var rightBottomPoint = points[0];
            var rightTopPoint = points[0];
            
            foreach (var point in points)
            {
                if (point.x < leftBottomPoint.x)
                {
                    leftBottomPoint.x = point.x;
                }
                else if (point.x > rightTopPoint.x)
                {
                    rightBottomPoint.x = point.x;
                    rightTopPoint.x = point.x;
                }
                if (point.y < leftBottomPoint.y)
                {
                    leftBottomPoint.y = point.y;
                    rightBottomPoint.y = point.y;
                }
                else if (point.y > rightTopPoint.y)
                {
                    rightTopPoint.y = point.y;
                }
            }
            
            rightBottomPoint.x += 2;
            rightBottomPoint.y -= 2;

            leftBottomPoint.x -= rightBottomPoint.x - leftBottomPoint.x;
            leftBottomPoint.y -= 2;
            
            rightTopPoint.x += 2;
            rightTopPoint.y += rightTopPoint.y - rightBottomPoint.y;
            
            
            
            var result = new Polygon();
            
            var edge0 = new DirectedEdge(leftBottomPoint, rightBottomPoint).SetPolygon(result);
            var edge1 = new DirectedEdge(rightBottomPoint, rightTopPoint).SetPolygon(result);
            var edge2 = new DirectedEdge(rightTopPoint, leftBottomPoint).SetPolygon(result);

            edge0.SetNext(edge1);
            edge1.SetNext(edge2);
            edge2.SetNext(edge0);

            result.SetFirstEdge(edge0, 3);
            return result;
        }

        private static readonly HashSet<DirectedEdge> TempEdgeHashSet = new();
        
        public static Polygon MergePolygon(List<Polygon> polygons)
        {
            TempEdgeHashSet.Clear();

            DirectedEdge currentEdge;
            foreach (var polygon in polygons)
            {
                currentEdge = polygon.FirstEdge;
                do
                {
                    TempEdgeHashSet.Add(currentEdge);
                    currentEdge = currentEdge.Next;
                } while (currentEdge != polygon.FirstEdge);
            }

            DirectedEdge firstEdge = null;
            foreach (var edge in TempEdgeHashSet)
            {
                if (!TempEdgeHashSet.Contains(edge.Twin))
                {
                    firstEdge = edge;
                    break;
                }
            }

            if (firstEdge == null)
            {
                return null;
            }

            var edgeCount = 0;
            DirectedEdge lastEdge = null;
            currentEdge = firstEdge;
            do
            {
                lastEdge?.SetNext(currentEdge);
                lastEdge = currentEdge;

                edgeCount++;
                
                while (true)
                {
                    if (!TempEdgeHashSet.Contains(currentEdge.Next.Twin))
                    {
                        currentEdge = currentEdge.Next;
                        break;
                    }
                    currentEdge = currentEdge.Next.Twin;
                }
            } while (currentEdge != firstEdge);
            lastEdge.SetNext(firstEdge);
            
            return new Polygon().SetFirstEdge(firstEdge, edgeCount);
        }

        public int SplitTriangle(Point point, List<Polygon> triangles)
        {
            if (triangles == null)
            {
                triangles = new List<Polygon>(EdgeCount);
            }
            else
            {
                triangles.Clear();
            }

            DirectedEdge lastEdge2 = null;
            var currentEdge = FirstEdge;
            do
            {
                var edge0 = new DirectedEdge(point, currentEdge.Start);
                var edge1 = currentEdge;
                var edge2 = new DirectedEdge(currentEdge.End, point);
                currentEdge = currentEdge.Next;

                var triangle = new Polygon().SetFirstEdge(edge0, 3);
                edge0.SetPolygon(triangle).SetNext(edge1);
                edge1.SetPolygon(triangle).SetNext(edge2);
                edge2.SetPolygon(triangle).SetNext(edge0);

                if (lastEdge2 != null)
                {
                    edge0.SetTwin(lastEdge2);
                    lastEdge2.SetTwin(edge0);
                }
                lastEdge2 = edge2;
                
                triangles.Add(triangle);
                
            } while (currentEdge != FirstEdge);

            var firstTriangleFirstEdge = triangles[0].FirstEdge;
            var lastTriangleLastEdge = triangles[^1].FirstEdge.Next.Next;

            firstTriangleFirstEdge.SetTwin(lastTriangleLastEdge);
            lastTriangleLastEdge.SetTwin(firstTriangleFirstEdge);

            return triangles.Count;
        }

        public bool ContainPoint(Point point)
        {
            var currentEdge = FirstEdge;
            do
            {
                if (currentEdge.Start == point || currentEdge.End == point)
                {
                    return true;
                }
                currentEdge = currentEdge.Next;
            } while (currentEdge != FirstEdge);

            return false;
        }

        public bool CheckInside(Point point)
        {
            float? lastZ = null;
            var currentEdge = FirstEdge;
            do
            {
                var vector0 = currentEdge.End - currentEdge.Start;
                var vector1 = point - currentEdge.Start;
                var z = vector0.x * vector1.y - vector0.y * vector1.x;
                if (lastZ != null && ((lastZ > 0 && z < 0) || (lastZ < 0 && z > 0)))
                {
                    return false;
                }
                lastZ = z;
                currentEdge = currentEdge.Next;
            } while (currentEdge != FirstEdge);

            return true;
        }

        public bool CheckCircumscribedCircleInside(Point point)
        {
            if (EdgeCount != 3)
            {
                throw new Exception("该多边形不是三角形，仅支持三角形求外接圆！");
            }

            var point0 = FirstEdge.Start;
            var point1 = FirstEdge.Next.Start;
            var point2 = FirstEdge.Next.Next.Start;
            
            var d = 2 * (point0.x * (point1.y - point2.y) + point1.x * (point2.y - point0.y) + point2.x * (point0.y - point1.y));
            var centerX = ((point0.x * point0.x + point0.y * point0.y) * (point1.y - point2.y) +
                           (point1.x * point1.x + point1.y * point1.y) * (point2.y - point0.y) +
                           (point2.x * point2.x + point2.y * point2.y) * (point0.y - point1.y)) / d;
            var centerY = ((point0.x * point0.x + point0.y * point0.y) * (point2.x - point1.x) +
                           (point1.x * point1.x + point1.y * point1.y) * (point0.x - point2.x) +
                           (point2.x * point2.x + point2.y * point2.y) * (point1.x - point0.x)) / d;
            var center = new Point(centerX, centerY);

            return (point - center).SqrMagnitude() <= (point0 - center).SqrMagnitude();
        }

        public void Draw()
        {
            var edge = FirstEdge;
            do
            {
                edge.Draw();
                edge = edge.Next;
            } while (edge != FirstEdge);
        }

        public override string ToString() => $"{FirstEdge.Start},{FirstEdge.Next.Start},{FirstEdge.Next.Next.Start}";
    }
}