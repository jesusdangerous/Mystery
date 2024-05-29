using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using MysteryWorld.Models.Enums;
using MysteryWorld.Models.Interfaces;
using MysteryWorld.Views;
using MysteryWorld.Controllers;

namespace MysteryWorld.Models;

public abstract class ItemModel : GameObjectView, IUsableItem
{
    private const int RegenerationStrength = 3;
    private const int RegenerationDuration = 5;

    private const float HealPotionCoefficient = 0.15f;

    private const float ItemLayer = 0.6f;

    [JsonProperty] public List<ItemEffects> Effects { get; private set; }

    protected ItemModel(Vector2 position, List<ItemEffects> effects) : base(position)
    {
        Effects = effects;
        CurrentSpriteId = SpriteId;
        LayerDepth = ItemLayer;
    }

    public void Use(CharacterController character)
    {
        foreach (var effect in Effects)
        {
            switch (effect)
            {
                case ItemEffects.HealingEffect:
                    var newHp = character.CurrentLifePoints + character.MaxLifePoints * HealPotionCoefficient;
                    character.CurrentLifePoints = (int)Math.Clamp(newHp, 1, character.MaxLifePoints);
                    break;
                case ItemEffects.TimedHealingEffect:
                    character.HealUp = TempModel.CreateTemporaryEffect(RegenerationDuration, RegenerationStrength);
                    break;
            }
        }
    }
}
