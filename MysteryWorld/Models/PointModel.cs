using System;
using System.Collections.Generic;


namespace MysteryWorld.Models
{
    public sealed class PointModel : IEquatable<PointModel>
    {
        public readonly double X;
        public readonly double Y;

        public PointModel(double x, double y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(PointModel other)
        {
            var admission = 0.00001f;
            if (other == null) return false;

            var deltaX = Math.Abs(X - other.X);
            var deltaY = Math.Abs(Y - other.Y);
            var newX = deltaX <= admission || deltaX <= Math.Max(Math.Abs(X), Math.Abs(other.X)) * admission;
            var newY = deltaY <= admission || deltaY <= Math.Max(Math.Abs(Y), Math.Abs(other.Y)) * admission;

            return newX && newY;
        }

        public double Distance(PointModel other)
        {
            const int i2 = 2;
            return Math.Sqrt(Math.Pow(X - other.X, i2) + Math.Pow(Y - other.Y, i2));
        }

        public bool IsInList(List<PointModel> points)
        {
            foreach (var point in points)
                if (Equals(point)) return true;
            return false;
        }

        public bool IsInArray(PointModel[] points)
        {
            foreach (var point in points)
                if (Equals(point))
                    return true;
            return false;
        }
    }
}