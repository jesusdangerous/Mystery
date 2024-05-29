using Microsoft.Xna.Framework;
using MysteryWorld.Models.Enums;
using MysteryWorld.Views;
using System.Collections.Generic;

namespace MysteryWorld.Models;

internal class TreasureChestModel : GameObjectView
{
    private const int Sprite = 3008;

    private const float ChestLayer = 0.8f;

    public readonly List<(DropAbleLoot type, int chance)> PossibleLoot = new() { (DropAbleLoot.Soul, 5), (DropAbleLoot.HealthPotion, 2) };
    public TreasureChestModel(Vector2 position) : base(position)
    {
        SpriteId = Sprite;
        CurrentSpriteId = SpriteId;
        isNotAnimated = true;
        LayerDepth = ChestLayer;
    }
}