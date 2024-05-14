namespace MysteryWorld.Models
{
    class Player
    {
        public int X { get; set; }
        public int Y { get; set; }

        public void SetPosition(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
