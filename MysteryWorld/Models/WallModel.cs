using Microsoft.Xna.Framework;
using MysteryWorld.Views;

namespace MysteryWorld.Models;

internal class WallModel : GameObjectView
{
    public int HitsLeft { get; set; }

    public WallModel(Vector2 position, int hits) : base(position)
    {
        HitsLeft = hits;
    }
}
