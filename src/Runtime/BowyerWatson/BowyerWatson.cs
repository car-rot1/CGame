using System.Collections.Generic;

namespace BowyerWatson
{
    public class BowyerWatson
    {
        private readonly IReadOnlyList<Point> _points;
        public List<Polygon> Triangles { get; }

        private readonly Point[] _superTrianglePoints;
        private readonly List<Polygon> _tempTriangleList;

        public BowyerWatson(IReadOnlyList<Point> points)
        {
            _points = points;
            _superTrianglePoints = new Point[3];
            Triangles = new List<Polygon>();
            _tempTriangleList = new List<Polygon>();
        }
        
        public void Init()
        {
            var superTriangle = Polygon.CreateSuperTriangle(_points);
            Triangles.Clear();
            _tempTriangleList.Clear();

            _superTrianglePoints[0] = superTriangle.FirstEdge.Start;
            _superTrianglePoints[1] = superTriangle.FirstEdge.Next.Start;
            _superTrianglePoints[2] = superTriangle.FirstEdge.Next.Next.Start;
            
            Triangles.Add(superTriangle);
        }

        public void Handle()
        {
            for (var i = 0; i < _points.Count; i++)
            {
                HandleItem(i);
            }
        }
        
        public void HandleItem(int index)
        {
            var point = _points[index];
            _tempTriangleList.Clear();
            for (var i = Triangles.Count - 1; i >= 0; i--)
            {
                var triangle = Triangles[i];
                if (triangle.CheckCircumscribedCircleInside(point))
                {
                    _tempTriangleList.Add(triangle);
                    Triangles.RemoveAt(i);
                }
            }
            var polygon = Polygon.MergePolygon(_tempTriangleList);
            polygon.SplitTriangle(point, _tempTriangleList);
            Triangles.AddRange(_tempTriangleList);

            if (index == _points.Count - 1)
            {
                for (var i = Triangles.Count - 1; i >= 0; i--)
                {
                    var triangle = Triangles[i];
                    foreach (var superTrianglePoint in _superTrianglePoints)
                    {
                        if (triangle.ContainPoint(superTrianglePoint))
                        {
                            Triangles.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
        }

        public bool Check(out Polygon failedTriangle, out Point failedPoint)
        {
            foreach (var triangle in Triangles)
            {
                foreach (var point in _points)
                {
                    if (!triangle.ContainPoint(point) && triangle.CheckCircumscribedCircleInside(point))
                    {
                        failedTriangle = triangle;
                        failedPoint = point;
                        return false;
                    }
                }
            }
            failedTriangle = default;
            failedPoint = default;
            return true;
        }

        public void Draw()
        {
            foreach (var triangle in Triangles)
            {
                triangle?.Draw();
            }
        }
    }
}
