using System.Linq;
using Microsoft.Xna.Framework;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using MysteryWorld.Models;
using MysteryWorld.Models.Enums;
using MysteryWorld.Views;

namespace MysteryWorld.Controllers;

public sealed class CollisionController
{
    private const int MainCharacters = 2;
    private const int TunerLow = 3;
    private const int TunerLowMid = 5;
    private const int TunerMidHigh = 7;
    private const int TunerHigh = 10;

    private const float ThresholdLow = 0.1f;
    private const float ThresholdMid = 0.3f;
    private const float ThresholdHigh = 0.6f;

    private readonly LevelController levelState;
    private readonly CombatController combatHandler;
    private int Performeter;
    private int NextStart;
    private int Tuner;

    public CollisionController(LevelController levelState, EventController eventDispatcher, CombatController combatHandler)
    {
        this.levelState = levelState;
        this.combatHandler = combatHandler;
        Performeter = 0;
        NextStart = 1;
        Tuner = TunerHigh;
    }

    public void ProcessCollisions(float deltaTime)
    {
        levelState.ActionForCharacters(character => ProcessCollidingObject(character, deltaTime));
        if (!levelState.IsInTechnicalState) return;
        Performeter = NextStart;
        NextStart = (NextStart + 1) % Tuner;
        if (NextStart == 0)
            AdjustTuningParameters();
    }

    private void AdjustTuningParameters()
    {
        var totalChars = levelState.FriendlySummons.Count + levelState.HostileSummons.Count + 2;
        var movingChars = levelState.FriendlySummons.Values.Where(summon => summon != null).Count(summon => summon.Path.Count != 0);
        movingChars += levelState.HostileSummons.Values.Where(summon => summon != null).Count(summon => summon.Path.Count != 0);
        movingChars += MainCharacters;

        Tuner = (movingChars / (float)totalChars) switch
        {
            < ThresholdLow => TunerLow,
            < ThresholdMid => TunerLowMid,
            < ThresholdHigh => TunerMidHigh,
            _ => TunerHigh
        };
    }

    private void ProcessCollidingObject(CharacterController character, float deltaTime)
    {
        if (levelState.IsInTechnicalState)
        {
            Performeter = (Performeter + 1) % Tuner;
            if (Performeter != 0) return;
        }
        var collidingObjects = levelState.QuadTree.Search(character.Hitbox);
        collidingObjects.AddRange(levelState.MapTree.Search(character.Hitbox));
        foreach (var obj in collidingObjects.Where(obj => obj.Id != character.Id))
        {
            switch (obj)
            {
                case CharacterController collidingCharacter:
                    ResolveCharacterCollision(character, collidingCharacter, deltaTime);
                    break;
                case ProjectileView projectile:
                    ResolveProjectileCollision(character, projectile);
                    break;
                case WallModel wall:
                    ResolveWallCollision(character, wall, deltaTime);
                    break;
                case TreasureChestModel treasureChest:
                    if (character is SummonerController)
                        ResolveTreasureChestCollision(treasureChest);
                    break;
            }
        }
    }

    private void ResolveCharacterCollision(GameObjectView character, GameObjectView collidingCharacter, float deltaTime)
    {
        if (levelState.MapTree.Search(collidingCharacter.Hitbox).Any()) return;
        var direction = collidingCharacter.Position - character.Position;
        if (direction == Vector2.Zero) direction = Vector2.One;
        direction.Normalize();
        collidingCharacter.Position += deltaTime * direction * GameController.ScaledPixelSize;
    }

    private void ResolveProjectileCollision(CharacterController character, ProjectileView projectile)
    {
        var radius = 2 * GameController.ScaledPixelSize;
        if (!(character.IsFriendly ^ projectile.IsFriendly)) return;
        if (character is SummonerController && projectile.Type == ProjectileType.HealSpell) return;
        if (character is SummonerController && levelState.IsInTechnicalState) return;
        if (!character.IsFriendly)
            character.SetColor(Color.Red);

        character.CurrentLifePoints -= (int)(projectile.Damage * projectile.Element.ElementalEffectiveness(character.Element));
        ApplyProjectileEffect(character, projectile.Effect, radius);
        projectile.State = InstanceState.LimitReached;

        if (!character.IsFriendly && character.CurrentState is CharacterState.Idle or CharacterState.Patrolling or CharacterState.ArchEnemyControl)
            combatHandler.SetTargetWithId(character.Id, projectile.CharacterId);

        if (projectile is ObstacleModel)
            levelState.AddToMutableUseAble(new EnvironmentIemController(projectile.Position,
                EnvironmentalAnimations.BearTrapSnapping, EnvironmentalMode.SingleAnimation));
    }

    private static void ResolveWallCollision(GameObjectView character, GameObjectView wall, float deltaTime)
    {
        var direction2 = character.Position - wall.Position;
        if (direction2 == Vector2.Zero)
            direction2 = Vector2.One;

        direction2.Normalize();
        character.Position += deltaTime * direction2 * GameController.ScaledPixelSize;
    }

    private void ResolveTreasureChestCollision(TreasureChestModel treasureChest)
    {
        foreach (var (type, _) in treasureChest.PossibleLoot)
        {
            ItemModel newItem;
            switch (type)
            {
                case DropAbleLoot.HealthPotion:
                    newItem = new HealthPotionModel(new Vector2(treasureChest.Position.X - 1 * GameController.ScaledPixelSize, treasureChest.Position.Y));
                    levelState.AddItem(newItem);
                    break;
            }
        }
        treasureChest.State = InstanceState.LimitReached;
        levelState.Summoner.StopMovement();
    }

    private void ApplyProjectileEffect(CharacterController character, ProjectileEffect projectileEffect, float radius)
    {
        character.UseProjectileEffect(projectileEffect, levelState.Summoner.HealingStrength, levelState.Summoner.SpeedStrength);
        var friendsInRange = levelState.QuadTree.SearchCharacters(new Rect(
            new Vector2(character.Position.X - radius / 2, character.Position.Y - radius / 2),
            new Vector2(radius, radius))).Where(c => c.IsFriendly).ToList();
        foreach (var friend in friendsInRange)
            if (Vector2.Distance(character.Position, friend.Position) < radius && character.Id != friend.Id && friend.Id != levelState.Summoner.Id)
                friend.UseProjectileEffect(projectileEffect, levelState.Summoner.HealingStrength, levelState.Summoner.SpeedStrength);
    }
}