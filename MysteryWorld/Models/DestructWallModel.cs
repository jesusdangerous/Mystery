using Microsoft.Xna.Framework;

namespace MysteryWorld.Models;

internal class DestructWallModel : WallModel
{
    public DestructWallModel(Vector2 position, int hits) : base(position, hits)
    {
        HitsLeft = hits;
    }
}
