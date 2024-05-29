using Microsoft.Xna.Framework;
using MysteryWorld.Views;

namespace MysteryWorld.Models;

public class BloodModel : GameObjectView
{
    private const float BloodShrineLayer = 0.8f;

    public BloodModel(Vector2 position) : base(position)
    {
        LayerDepth = BloodShrineLayer;
    }
}