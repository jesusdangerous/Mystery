using Microsoft.Xna.Framework;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace MysteryWorld.Models
{
    public sealed class Rect
    {
        internal Vector2 Position;
        internal Vector2 Size;
        
        public Rect(Vector2 pos, Vector2 size)
        {
            Position = pos;
            Size = size;
        }

        public Rect(Rectangle hitbox)
        {
            Position = new Vector2(hitbox.Left, hitbox.Top);
            Size = new Vector2(hitbox.Width, hitbox.Height);
        }

        internal bool Contains(Rect rect) =>
            rect.Position.X >= Position.X && rect.Position.Y >= Position.Y &&
                    rect.Position.X + rect.Size.X <= Position.X + Size.X && rect.Position.Y + rect.Size.Y <= Position.Y + Size.Y;

        internal bool Overlaps(Rect rect) =>
            Position.X < rect.Position.X + rect.Size.X && Position.X + Size.X >= rect.Position.X &&
                    Position.Y < rect.Position.Y + rect.Size.Y && Position.Y + Size.Y >= rect.Position.Y;
    }
}