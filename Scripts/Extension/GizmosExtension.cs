using UnityEngine;

namespace CGame
{
    public static class GizmosExtension
    {
        public static void DrawRect(Rect rect)
        {
            var points = new Vector3[]
            {
                new(rect.xMin, -rect.yMin),
                new(rect.xMin, -rect.yMax),
                new(rect.xMax, -rect.yMax),
                new(rect.xMax, -rect.yMin)
            };
            for (var i = 0; i < points.Length; i++)
            {
                var nextIndex = (i + 1) % points.Length;
                Gizmos.DrawLine(points[i], points[nextIndex]);
            }
        }
        
        public static void DrawRect(RectInt rect)
        {
            var points = new Vector3[]
            {
                new(rect.xMin, -rect.yMin),
                new(rect.xMin, -rect.yMax),
                new(rect.xMax, -rect.yMax),
                new(rect.xMax, -rect.yMin)
            };
            for (var i = 0; i < points.Length; i++)
            {
                var nextIndex = (i + 1) % points.Length;
                Gizmos.DrawLine(points[i], points[nextIndex]);
            }
        }

        public static void DrawCircle(Vector2 point, float radius, int segments = 100)
        {
            DrawCapsule(point, new Vector2(radius, radius), CapsuleDirection2D.Horizontal, segments);
        }

        public static void DrawCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction = CapsuleDirection2D.Horizontal, int segments = 100)
        {
            if (segments > 1000_000)
                segments = 1000_000;
            
            var angle = 0f;
            var angleOffset = 360f / segments;
            if (direction is CapsuleDirection2D.Vertical)
                (size.x, size.y) = (size.y, size.x);
            var startPoint = point + new Vector2(size.x, 0);

            var lastPoint = startPoint;
            for (var i = 1; i <= segments - 1; i++)
            {
                angle += angleOffset;
                var nowPoint = point + new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle) * size.x, Mathf.Sin(Mathf.Deg2Rad * angle) * size.y);
                Gizmos.DrawLine(lastPoint, nowPoint);
                lastPoint = nowPoint;
            }
            
            Gizmos.DrawLine(lastPoint, startPoint);
        }

        public static void DrawCurve(Vector2 point, AnimationCurve curve, Vector2? scale = null, int segments = 100, Color? curveColor = null)
        {
            if (curve == null || curve.keys.Length == 0)
                return;
            
            if (segments > 1000_000)
                segments = 1000_000;
            
            scale ??= Vector2.one;
            var defaultColor = Gizmos.color;
            curveColor ??= defaultColor;
            
            Gizmos.color = curveColor.Value;
            var value = curve.keys[0].time;
            var valueOffset = curve.GetTime() / segments;
            var lastPoint = curve.GetFirstPoint();
            for (var i = 1; i <= segments; i++)
            {
                value += valueOffset;
                var nowYPoint = new Vector2(value, curve.Evaluate(value));
                
                Gizmos.DrawLine(point + new Vector2(lastPoint.x * scale.Value.x, lastPoint.y * scale.Value.y), point + new Vector2(nowYPoint.x * scale.Value.x, nowYPoint.y * scale.Value.y));
                lastPoint = nowYPoint; 
            }
            Gizmos.color = defaultColor;
        }
        
        public static void DrawXYCurve(Vector2 point, AnimationCurve curve, Vector2? scale = null, int segments = 100, Color? xCurveColor = null, Color? yCurveColor = null)
        {
            if (curve == null || curve.keys.Length == 0)
                return;
            
            if (segments > 1000_000)
                segments = 1000_000;

            scale ??= Vector2.one;
            var defaultColor = Gizmos.color;
            xCurveColor ??= Color.red;
            yCurveColor ??= Color.blue;

            var realLength = curve.GetCurveLength(segments);
            
            var value = curve.keys[0].time;
            var valueOffset = curve.GetTime() / segments;
            
            var lastYPoint = curve.GetFirstPoint();
            var lastXPoint = new Vector2(0, value);
            var nowLength = 0f;
            for (var i = 1; i <= segments; i++)
            {
                value += valueOffset;
                var nowYPoint = new Vector2(value, curve.Evaluate(value));
                nowLength += (nowYPoint - lastYPoint).magnitude;
                Gizmos.color = yCurveColor.Value;
                Gizmos.DrawLine(point + new Vector2(lastYPoint.x * scale.Value.x, lastYPoint.y * scale.Value.y), point + new Vector2(nowYPoint.x * scale.Value.x, nowYPoint.y * scale.Value.y));
                lastYPoint = nowYPoint;
                
                var nowXPoint = new Vector2(nowLength / realLength, value);
                Gizmos.color = xCurveColor.Value;
                Gizmos.DrawLine(point + new Vector2(lastXPoint.x * scale.Value.x, lastXPoint.y * scale.Value.y), point + new Vector2(nowXPoint.x * scale.Value.x, nowXPoint.y * scale.Value.y));
                lastXPoint = nowXPoint;
            }
            Gizmos.color = defaultColor;
        }
    }
}