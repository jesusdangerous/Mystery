namespace MysteryWorld.Models
{
    class Box
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int NewX { get; set; }
        public int NewY { get; set; }

        public bool Moved { get; set; } = false;

        public Box(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void Set(int x, int y)
        {
            NewX = x;
            NewY = y;
            Moved = true;
        }

        public void Move()
        {
            X = NewX;
            Y = NewY;
            Moved = false;
        }
    }
}
