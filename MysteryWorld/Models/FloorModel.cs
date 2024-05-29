namespace MysteryWorld.Models
{
    public sealed class FloorModel
    {
        internal readonly PointModel start;
        internal readonly PointModel end;

        internal FloorModel(PointModel start, PointModel end)
        {
            this.start = start;
            this.end = end;
        }
    }
}