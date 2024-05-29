using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MysteryWorld.Controllers;
using MysteryWorld.Models.Enums;

namespace MysteryWorld.Models;

public abstract class MainCharacterModel : CharacterController
{
    public int MaxXp { get; protected set; }
    public int AccumulatedXp { get; protected set; }
    public int XpLevel { get; protected set; } = 1;
    public int MaxMana { get; protected set; }
    public int CurrentMana { get; set; }
    public Dictionary<ElementType, int> Skills { get; set; }

    protected MainCharacterModel(Vector2 position) : base(position)
    {
    }

    protected int McAddIntScaling(int baseVal, float increment) =>
        (int)(baseVal + increment * (XpLevel - 1));

    protected float McAddFloatScaling(float baseVal, float increment) =>
        baseVal + increment * (XpLevel - 1);
}