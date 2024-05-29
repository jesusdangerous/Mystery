using Microsoft.Xna.Framework;
using MysteryWorld.Views;

namespace MysteryWorld.Models
{
    public sealed class LadderModel : GameObjectView
    {
        private const int Sprite = 3072;
        private const float LadderLayer = 0.8f;

        public LadderModel(Vector2 position) : base(position)
        {
            SpriteId = Sprite;
            CurrentSpriteId = SpriteId;
            LayerDepth = LadderLayer;
        }
    }
}
