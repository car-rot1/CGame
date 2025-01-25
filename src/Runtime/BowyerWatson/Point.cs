using System;
using System.Runtime.CompilerServices;

namespace BowyerWatson
{
    [Serializable]
    public struct Point : IEquatable<Point>
    {
        public float x;
        public float y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Point(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Point other) => Math.Abs(x - other.x) < 1e-10 && Math.Abs(y - other.y) < 1e-10;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj) => obj is Point other && Equals(other);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => x.GetHashCode() ^ y.GetHashCode() << 2;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Point a, Point b) => a.Equals(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Point a, Point b) => !a.Equals(b);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point operator +(Point a, Point b) => new(a.x + b.x, a.y + b.y);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point operator -(Point a, Point b) => new(a.x - b.x, a.y - b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float SqrMagnitude() => x * x + y * y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => $"({x}, {y})";
    }
}