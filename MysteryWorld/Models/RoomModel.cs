using System;
using System.Collections.Generic;
using System.Linq;

namespace MysteryWorld.Models
{
    public sealed class RoomModel
    {
        internal readonly PointModel Middle;
        internal readonly int Width;
        internal readonly int Height;
        internal readonly PointModel TopLeftCorner;
        private readonly PointModel[] corners;

        internal RoomModel(PointModel mid, int width, int height)
        {
            Middle = mid;
            Width = width;
            Height = height;
            const float f2 = 2f;
            TopLeftCorner = new PointModel(mid.X - Math.Floor(width / f2), mid.Y - Math.Floor(height / f2));
            var topRightCorner = new PointModel(mid.X + Math.Floor(width / f2), mid.Y - Math.Floor(height / f2));
            var bottomLeftCorner = new PointModel(mid.X - Math.Floor(width / f2), mid.Y + Math.Floor(height / f2));
            var bottomRightCorner = new PointModel(mid.X + Math.Floor(width / f2), mid.Y + Math.Floor(height / f2));
            corners = new[] { TopLeftCorner, topRightCorner, bottomLeftCorner, bottomRightCorner };
        }

        internal bool Overlaps(RoomModel other) =>
            Middle.Distance(TopLeftCorner) + 5 * Math.Sqrt(2) >= other.corners.Select(point => Middle.Distance(point)).Prepend(double.PositiveInfinity).Min();

        internal void FloorConnect(RoomModel other, List<FloorModel> horizontal, List<FloorModel> vertical)
        {
            const double tolerance = 0.00001f;
            if (Math.Abs(Middle.Y - other.Middle.Y) <= tolerance || Math.Abs(Middle.Y - other.Middle.Y) <= Math.Max(Math.Abs(Middle.Y), Math.Abs(other.Middle.Y)) * tolerance)
                horizontal.Add(new FloorModel(Middle, other.Middle));
            else
            {
                if (Math.Abs(Middle.X - other.Middle.X) <= tolerance || Math.Abs(Middle.X - other.Middle.X) <= Math.Max(Math.Abs(Middle.X), Math.Abs(other.Middle.X)) * tolerance)
                    vertical.Add(new FloorModel(Middle, other.Middle));
                else
                {
                    horizontal.Add(new FloorModel(Middle, new PointModel(other.Middle.X, Middle.Y)));
                    vertical.Add(new FloorModel(new PointModel(other.Middle.X, Middle.Y), other.Middle));
                }
            }
        }
    }
}