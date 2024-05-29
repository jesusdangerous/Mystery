#nullable enable
using System.Collections.Generic;
using System.Linq;
using MysteryWorld.Controllers;
using MysteryWorld.Models.Enums;
using MysteryWorld.Models.Interfaces;
using MysteryWorld.Views;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace MysteryWorld.Models;

public sealed class GameModel
{
    private const int ProjectileOffset = 20;
    private const int FireBallCost = 20;

    private readonly LevelController LevelState;
    private readonly EventController EventDispatcher;
    private readonly EnemyBehaviourController AiHandler;
    private readonly CollisionController CollisionHandler;
    private readonly IPathfinder Pathfinder;
    private readonly CombatController CombatHandler;

    public GameModel(LevelController levelState, EventController eventDispatcher)
    {
        LevelState = levelState;
        EventDispatcher = eventDispatcher;
        AiHandler = new EnemyBehaviourController(levelState);
        Pathfinder = new PathfinderController(levelState);
        CombatHandler = new CombatController(levelState, Pathfinder, eventDispatcher);
        CollisionHandler = new CollisionController(levelState, eventDispatcher, CombatHandler);
    }

    internal void Update(float deltaTime)
    {
        AiHandler.Update(LevelState, this);
        CollisionHandler.ProcessCollisions(deltaTime);
        CombatHandler.Update();
    }

    internal void ConsumeItem(MainCharacterModel mainCharacter)
    {
        var items = LevelState.QuadTree.SearchItems(mainCharacter.Hitbox);
        if (items.Any()) LevelState.ConsumeItem(items[0]);
    }

    internal void SelectCharacter(CharacterController character)
    {
        character.Selected = !character.Selected;
    }

    internal void MoveCharacter(CharacterController character, Vector2 destination)
    {
        CombatHandler.RemoveTarget(character.Id);
        var path = Pathfinder.CalculatePath(character.Position.ToGrid(), destination.ToGrid());
        character.SetPath(path);
    }

    internal void MoveCharacters(List<CharacterController> characters, Vector2 destination)
    {
        foreach (var character in characters)
            CombatHandler.RemoveTarget(character.Id);
        PathExtensions.MultiP(LevelState, Pathfinder, characters, destination);
    }

    internal void SummonFriendlyMonster(SummonType? type, Vector2 position)
    {
        if (!LevelState.Summoner.IsCanSummon() || !LevelState.Summoner.CanSpawnCharacterInRange(position, (int)(LevelState.Summoner.SummonRange * GameController.ScaledPixelSize)) ||
            !LevelState.IsSpaceFree(position) || !LevelState.OnGround(position))
            return;

        position = CameraController.TileCenterToWorld(position.ToGrid());
        EnvironmentIemController? leftOfSummoner = null;
        EnvironmentIemController? topOfSummoner = null;
        EnvironmentIemController? rightOfSummoner = null;
        EnvironmentIemController? bottomOfSummoner = null;

        LevelState.AddToMutableUseAble(new List<GameObjectView> { leftOfSummoner!, topOfSummoner!, rightOfSummoner!, bottomOfSummoner! });
    }

    internal void ShootFireball(Vector2 mousePosition)
    {
        if (LevelState.Summoner.CurrentMana < FireBallCost || LevelState.Summoner.FireBallCoolDown < SummonerController.CoolDownLimitFire)
        {
            return;
        }

        LevelState.Summoner.FireBallCoolDown = 0;
        LevelState.Summoner.ActualMana -= FireBallCost;

        var direction = mousePosition - LevelState.Summoner.Position;
        direction.Normalize();
        var start = LevelState.Summoner.Position + ProjectileOffset * direction;
        var destination = LevelState.Summoner.Position + (LevelState.Summoner.Range + 4 * GameController.ScaledPixelSize) * direction;
        var fireball = new FireballModel(start, destination, true, LevelState.Summoner.Id, LevelState.Summoner.FireballDamage, ElementType.Neutral);
        LevelState.AddProjectile(fireball);
    }

    internal void AttackCharacter(CharacterController attacker, CharacterController target)
    {
        CombatHandler.SelectTarget(attacker.Id, target);
        CombatHandler.MoveAttackerToTarget(attacker, target.Position);
    }

    internal CharacterController? FindTarget(Vector2 position)
    {
        var targets = LevelState.QuadTree.PointSearchCharacters(position);
        return targets.FirstOrDefault();
    }
}