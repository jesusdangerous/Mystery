namespace MysteryWorld.Models
{
    public sealed class TempModel
    {
        public int Active { get; set; }
        public float Strength { get; set; }
        public int Duration { get; set; }
        public double Time { get; set; }

        internal void Update(float deltaTime)
        {
            if (Active == 0) return;
            Time += deltaTime;
            if (Time >= Duration)
                Active = 0;
        }

        internal static TempModel CreateTemporaryEffect(int duration, float strength)
        {
            return new TempModel
            {
                Active = 1,
                Duration = duration,
                Strength = strength,
                Time = 0
            };
        }
    }
}