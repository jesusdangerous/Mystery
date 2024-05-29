using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using MysteryWorld.Models.Enums;
using MysteryWorld.Models.Interfaces;
using MysteryWorld.Models;

namespace MysteryWorld.Controllers;

internal sealed class PaladinController : SummonModel, IEnemyBehaviour
{
    private const int Sprite = 1792;
    private const int HpBase = 800;
    private const int HpScale = 100;
    private const int DamageBase = 100;
    private const int DamageScale = 6;
    private const int DelayBase = 500;

    private const float GridSpeedBase = 2.5f;
    private const float GridRange = 1f;
    private const float GridVision = 5f;

    private IEnemyBehaviour behaviour;

    public PaladinController(Vector2 position, int level, bool friendly = false) : base(position, level, friendly)
    {
        Level = level;
        SpriteId = Sprite;
        CurrentSpriteId = SpriteId;
        MaxLifePoints = AddIntScaling(HpBase, HpScale);
        CurrentLifePoints = MaxLifePoints;
        Damage = AddIntScaling(DamageBase, DamageScale);
        Velocity = GridSpeedBase * GameController.ScaledPixelSize;
        Delay = DelayBase;
        Element = ElementType.Lightning;
        IsFriendly = friendly;
        CombatType = CombatType.Melee;
        Range = GridRange * GameController.ScaledPixelSize;
        Vision = GridVision * GameController.ScaledPixelSize;
        CurrentState = CharacterState.Patrolling;
        behaviour = new KnightBehaviourController(this);
    }

    public void UpdateState(LevelController levelState, GameModel gameLogic)
    {
        behaviour.UpdateState(levelState, gameLogic);
    }

    public void Initialize(CharacterController enemy)
    {
        behaviour.Initialize(enemy);
    }
}