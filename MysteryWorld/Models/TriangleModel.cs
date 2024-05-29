using System;
using System.Collections.Generic;

namespace MysteryWorld.Models
{
    public sealed class TriangleModel : IEquatable<TriangleModel>
    {
        internal readonly PointModel PointA;
        internal readonly PointModel PointB;
        internal readonly PointModel PointC;
        internal readonly PointModel[] Points;
        internal readonly EdgeModel[] Edges;
        private PointModel circumcenter;

        public TriangleModel(PointModel pointA, PointModel pointB, PointModel pointC)
        {
            var edgeAB = new EdgeModel(pointA, pointB);
            var edgeAC = new EdgeModel(pointA, pointC);
            var edgeBC = new EdgeModel(pointB, pointC);
            PointA = pointA;
            PointB = pointB;
            PointC = pointC;
            Points = new[] { pointA, pointB, pointC };
            Edges = new[] { edgeBC, edgeAC, edgeAB };
            circumcenter = CalculateCircumCircle();
        }

        public bool Equals(TriangleModel other)
        {
            var result = true;
            if (other == null) return true;

            foreach (var point in other.Points)
                if (!point.IsInArray(Points)) result = false;
            return result;
        }

        public bool IsInList(List<TriangleModel> triangles)
        {
            var alternative = new List<TriangleModel>
            {
                new(PointA, PointB, PointC),
                new(PointA, PointC, PointB),
                new(PointB, PointA, PointC),
                new(PointB, PointC, PointA),
                new(PointC, PointA, PointB),
                new(PointC, PointB, PointA)
            };
            foreach (var triangle in alternative)
                foreach (var triangle2 in triangles)
                    if (triangle.Equals(triangle2)) return true;
            return false;
        }

        private PointModel CalculateCircumCircle()
        {
            const double admission = 0.00001f;

            var firstPoint = PointA;
            var secondPoint = PointB;
            var thirdPoint = PointC;

            var diff13 = Math.Abs(PointB.Y - PointC.Y);
            var diff23 = Math.Abs(PointA.Y - PointC.Y);

            if (diff23 <= admission || diff23 <= Math.Max(Math.Abs(PointB.Y), Math.Abs(PointC.Y)) * admission)
            {
                secondPoint = PointC;
                thirdPoint = PointB;
            }

            if (diff13 <= admission || diff13 <= Math.Max(Math.Abs(PointB.Y), Math.Abs(PointC.Y)) * admission)
            {
                firstPoint = PointC;
                thirdPoint = PointA;
            }

            var n1 = -(secondPoint.X - thirdPoint.X) / (secondPoint.Y - thirdPoint.Y);
            var n2 = -(firstPoint.X - thirdPoint.X) / (firstPoint.Y - thirdPoint.Y);
            var x1 = 1 / (2 * (n1 - n2));
            var x2 = (Math.Pow(firstPoint.X, 2) - Math.Pow(thirdPoint.X, 2)) / (firstPoint.Y - thirdPoint.Y);
            var x3 = (Math.Pow(secondPoint.X, 2) - Math.Pow(thirdPoint.X, 2)) / (secondPoint.Y - thirdPoint.Y);
            var x = x1 * (x2 - x3 + firstPoint.Y - secondPoint.Y);
            var y = (n1 * (2 * x - secondPoint.X - thirdPoint.X) + secondPoint.Y + thirdPoint.Y) / 2;
            return new PointModel(x, y);
        }

        internal bool InCircle(PointModel p)
        {
            circumcenter = CalculateCircumCircle();
            return circumcenter.Distance(p) <= circumcenter.Distance(PointA);
        }
    }
}