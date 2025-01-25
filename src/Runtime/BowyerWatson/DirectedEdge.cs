using UnityEngine;

namespace BowyerWatson
{
    public class DirectedEdge
    {
        public Point Start { get; private set; }
        public Point End { get; private set; }
        public DirectedEdge Next { get; private set; }
        public DirectedEdge Twin { get; private set; }
        public Polygon Polygon { get; private set; }

        public DirectedEdge(Point start, Point end)
        {
            Start = start;
            End = end;
        }

        #region SetValue
        
        public DirectedEdge SetPoint(Point start, Point end)
        {
            Start = start;
            End = end;
            return this;
        }

        public DirectedEdge SetNext(DirectedEdge edge)
        {
            Next = edge;
            return this;
        }

        public DirectedEdge SetTwin(DirectedEdge edge)
        {
            Twin = edge;
            return this;
        }

        public DirectedEdge SetPolygon(Polygon polygon)
        {
            Polygon = polygon;
            return this;
        }
        
        #endregion

        public void Draw()
        {
            Gizmos.DrawLine(new Vector2(Start.x, Start.y), new Vector2(End.x, End.y));
        }
        
        public override string ToString() => $"{Start}->{End}";
    }
}