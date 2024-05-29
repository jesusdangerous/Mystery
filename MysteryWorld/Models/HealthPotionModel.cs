using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MysteryWorld.Models.Enums;

namespace MysteryWorld.Models;

internal sealed class HealthPotionModel : ItemModel
{
    private const int Sprite = 2952;

    public HealthPotionModel(Vector2 position) : base(position,
        new List<ItemEffects>() { ItemEffects.HealingEffect })
    {
        SpriteId = Sprite;
        CurrentSpriteId = SpriteId;
    }
}