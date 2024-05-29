using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MysteryWorld.Models
{
    internal sealed class FrameCounterModel
    {
        private const int BufferLength = 8;
        private const int WantedAverage = 60;

        private readonly Stopwatch Stopwatch;
        private readonly Queue<long> Queue;
        private readonly int Length;

        internal FrameCounterModel()
        {
            Stopwatch = new Stopwatch();
            Length = BufferLength;
            Queue = new Queue<long>(Length);
            for (var i = 0; i < Length; i++)
                Queue.Enqueue(WantedAverage);

            Stopwatch.Start();
        }

        internal void Update()
        {
            Queue.Dequeue();
            var sec = Stopwatch.Elapsed.TotalSeconds;
            var frames = (long)(1 / sec);
            Queue.Enqueue(frames < 0 ? 0 : frames);
            Stopwatch.Restart();
        }

        private int FramesPerSecond() =>
            (int)Queue.Sum() / Length;

        internal void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(spriteFont,
                $"FPS: {FramesPerSecond()}",
                new Vector2(1, 1),
                Color.Green);
            spriteBatch.End();
        }
    }
}