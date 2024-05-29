#nullable enable
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MysteryWorld.Models;
using MysteryWorld.Models.Enums;
using MysteryWorld.Models.Interfaces;

namespace MysteryWorld.Controllers;

public sealed class ArchenemyController : MainCharacterModel, IEnemyBehaviour
{
    private const int Sprite = 1088;
    private const int Experience = 100;
    private const int Mana = 10;
    private const int HpBase = 2000;
    private const int HpScale = 500;
    private const int DamageBase = 60;
    private const int DamageScale = 8;
    private const int DelayBase = 110;

    private const float GridSpeedBase = 4f;
    private const float GridSpeedScale = 0.1f;
    private const float GridRange = 3f;
    private const float GridVision = 7f;

    private readonly IEnemyBehaviour behaviour;

    public ArchenemyController(Vector2 position, int unitLevel, IEnemyBehaviour? behaviour) : base(position)
    {
        SpriteId = Sprite;
        CurrentSpriteId = SpriteId;
        MaxXp = Experience;
        MaxMana = Mana;
        CurrentMana = Mana;
        MaxLifePoints = McAddIntScaling(HpBase, HpScale);
        CurrentLifePoints = MaxLifePoints;
        Damage = McAddIntScaling(DamageBase, DamageScale);
        Delay = DelayBase;
        Velocity = McAddFloatScaling(GridSpeedBase, GridSpeedScale) * GameController.ScaledPixelSize;
        Range = GridRange * GameController.ScaledPixelSize;
        Vision = GridVision * GameController.ScaledPixelSize;
        Element = ElementType.Neutral;
        IsFriendly = false;
        CurrentState = CharacterState.Patrolling;
        CombatType = CombatType.Melee;

        Skills = new Dictionary<ElementType, int>
        {
            { ElementType.Magic, unitLevel },
            { ElementType.Fire, unitLevel },
            { ElementType.Ghost, unitLevel },
            { ElementType.Lightning, unitLevel }
        };
        this.behaviour = behaviour ?? new BanditEnemyController(this);
    }

    public void UpdateState(LevelController levelState, GameModel gameLogic)
    {
        behaviour.UpdateState(levelState, gameLogic);
    }

    public void Initialize(CharacterController enemy)
    {
        behaviour.Initialize(enemy);
    }

    public CharacterState GetState() =>
        CurrentState;
}