using System.Security.Cryptography;
using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MysteryWorld.Models;
using MysteryWorld.Models.Enums;
using MysteryWorld.Models.Interfaces;

namespace MysteryWorld.Controllers;

public sealed class BanditEnemyController : IEnemyBehaviour
{
    private CharacterState EnemyState => Enemy.CurrentState;
    private ArchenemyController Enemy;

    private bool isAbleToWalk, isAbleToSummon, isAbleToThrowTrap, isAbleToFlee;
    private readonly List<string> controlledSummonIds;
    private readonly Timer walkTimerInterval, summonTimerInterval, bearTrapTimer;
    private int summonCapacity, bearTrapCount;

    private const int UnitThreshold = 5;
    private const int BearTrapThreshold = 10;
    private const int WalkTimer = 5000;
    private const int SummonTimer = 4000;
    private const int BearTrapTimer = 5000;
    private const int BearTrapDamage = 100;

    private const float FlightResponseHp = 0.2f;
    private const float FightOnEnemyHp = 0.5f;

    private const int ElementalPriority = 3;
    private const int CombatTypePriority = 2;
    private const int HealthPriority = 1;

    private const float HpEvaluationThreshold = 0.5f;


    public BanditEnemyController(ArchenemyController enemy)
    {
        Enemy = enemy;
        isAbleToFlee = true;
        summonCapacity = Math.Min(enemy.Skills[ElementType.Fire], 3);
        bearTrapCount = BearTrapThreshold;
        controlledSummonIds = new List<string>();

        isAbleToWalk = true;
        walkTimerInterval = new Timer(WalkTimer) { AutoReset = true };
        walkTimerInterval.Elapsed += WalkTimerElapsed;
        walkTimerInterval.Start();

        isAbleToSummon = true;
        summonTimerInterval = new Timer(SummonTimer) { AutoReset = true };
        summonTimerInterval.Elapsed += SummonTimerElapsed;

        isAbleToThrowTrap = true;
        bearTrapTimer = new Timer(BearTrapTimer) { AutoReset = true };
        bearTrapTimer.Elapsed += BearTrapTimerElapsed;
    }

    public void UpdateState(LevelController levelState, GameModel gameLogic)
    {
        UpdateControlledSummonList(levelState);

        if (controlledSummonIds.Count <= UnitThreshold && Enemy.CurrentState != CharacterState.Fleeing)
            RecruitHostileSummons(levelState);

        switch (EnemyState)
        {
            case CharacterState.Idle:
                break;
            case CharacterState.Patrolling:
                Patrolling(levelState, gameLogic);
                break;
            case CharacterState.Attacking:
                Attacking(levelState, gameLogic);
                break;
            case CharacterState.Fleeing:
                Fleeing(levelState, gameLogic);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Fleeing(LevelController levelState, GameModel gameLogic)
    {
        if (isAbleToThrowTrap && bearTrapCount > 0)
        {
            levelState.AddProjectile(new ObstacleModel(Enemy.Position, new Vector2(0, 0), Enemy.IsFriendly, Enemy.Id, BearTrapDamage, ElementType.Neutral));
            bearTrapCount--;
            isAbleToThrowTrap = false;
        }

        if (Enemy.movementState != MovementState.Idle) return;

        bearTrapTimer.Stop();
        Enemy.CurrentState = CharacterState.Patrolling;
        GatherControlledSummons(levelState, gameLogic);
    }

    private void Attacking(LevelController levelState, GameModel gameLogic)
    {
        var strategy = DetermineCombatStrategy(levelState);
        switch (strategy)
        {
            case ConflictResolutionStrategy.FightOn:
                ExecuteCombatStrategy(levelState, gameLogic);
                break;
            case ConflictResolutionStrategy.AbandonFight:
                if (isAbleToFlee)
                {
                    AbandonControlledSummons(levelState, gameLogic);
                    Enemy.CurrentState = CharacterState.Fleeing;
                    isAbleToFlee = false;
                    summonTimerInterval.Stop();
                    bearTrapTimer.Start();
                }
                else ExecuteCombatStrategy(levelState, gameLogic);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Patrolling(LevelController levelState, GameModel gameLogic)
    {
        if (IsTargetInRange(levelState))
        {
            Enemy.CurrentState = CharacterState.Attacking;
            summonTimerInterval.Start();
        }
        if (isAbleToWalk) MoveToRandomAdjacentTile(levelState, gameLogic);
    }

    private ConflictResolutionStrategy DetermineCombatStrategy(LevelController levelState)
    {
        if (Enemy.CurrentLifePoints < Enemy.MaxLifePoints * FlightResponseHp)
            return ConflictResolutionStrategy.AbandonFight;

        return summonCapacity switch
        {
            0 when controlledSummonIds.Count == 0 => ConflictResolutionStrategy.AbandonFight,
            > 0 when Enemy.CurrentLifePoints >= FightOnEnemyHp * Enemy.MaxLifePoints => ConflictResolutionStrategy.FightOn,
            _ => !EvaluateChanceToWin(levelState) ? ConflictResolutionStrategy.AbandonFight : ConflictResolutionStrategy.FightOn
        };
    }

    private void ExecuteCombatStrategy(LevelController levelState, GameModel gameLogic)
    {
        if (UnitThreshold - controlledSummonIds.Count > 0 && isAbleToSummon && summonCapacity > 0)
            CreateAndAddHostileSummon(levelState);

        var controlledSummons = GetControlledSummons(levelState);
        var targetsInVision = GetGroupTargets(levelState);

        if (!MemberInCombat(levelState) && targetsInVision.Count == 0)
        {
            Enemy.CurrentState = CharacterState.Patrolling;
            GatherControlledSummons(levelState, gameLogic);
            return;
        }

        if (targetsInVision.Count == 0) return;

        gameLogic.AttackCharacter(Enemy, SelectPriorityTarget(Enemy, targetsInVision));
        foreach (var controlledSummon in controlledSummons)
            gameLogic.AttackCharacter(controlledSummon, SelectPriorityTarget(controlledSummon, targetsInVision));
    }

    private static CharacterController SelectPriorityTarget(CharacterController controlledSummon, List<CharacterController> targets) =>
        targets.MaxBy(target => Priority(controlledSummon, target));

    private static int Priority(CharacterController controlledSummon, CharacterController potentialTarget)
    {
        var priority = 0;
        if (controlledSummon.Element.ElementIsEffective(potentialTarget.Element))
            priority += ElementalPriority;

        switch (controlledSummon.CombatType)
        {
            case CombatType.Ranged when potentialTarget.CombatType == CombatType.Melee:
            case CombatType.Melee or CombatType.AoE or CombatType.SelfDamage when potentialTarget.CombatType == CombatType.Ranged:
                priority += CombatTypePriority;
                break;
        }

        if (controlledSummon.CurrentLifePoints > potentialTarget.CurrentLifePoints)
            priority += HealthPriority;

        return priority;
    }

    private bool EvaluateChanceToWin(LevelController levelState)
    {
        var controlledSummons = GetControlledSummons(levelState);
        if (controlledSummons.Count == 0) return false;

        return controlledSummons.Sum(summon => (float)summon.CurrentLifePoints / summon.MaxLifePoints) >= HpEvaluationThreshold;
    }

    private void MoveToRandomAdjacentTile(LevelController levelState, GameModel gameLogic)
    {
        if (Enemy.movementState == MovementState.Moving) return;

        var gridCoordinates = Enemy.Position.ToGrid();
        var neighbors = levelState.GameMap.PassableNeighbors(gridCoordinates).ToList();
        var randomInt = RandomNumberGenerator.GetInt32(0, neighbors.Count);
        gameLogic.MoveCharacter(Enemy, CameraController.TileCenterToWorld(neighbors[randomInt]));
        GatherControlledSummons(levelState, gameLogic);
        isAbleToWalk = false;
    }

    private void GatherControlledSummons(LevelController levelState, GameModel gameLogic)
    {
        var controlledSummons = GetControlledSummons(levelState);
        var gridCoordinates = Enemy.Position.ToGrid();
        var neighbors = levelState.GameMap.PassableNeighbors(gridCoordinates).ToList();
        foreach (var controlledSummon in controlledSummons)
        {
            var randomInt = RandomNumberGenerator.GetInt32(0, neighbors.Count);
            gameLogic.MoveCharacter(controlledSummon, CameraController.TileCenterToWorld(neighbors[randomInt]));
        }
    }

    private void AbandonControlledSummons(LevelController levelState, GameModel gameLogic)
    {
        var fleeTarget = (from i in levelState.HostileSummons where !controlledSummonIds.Contains(i.Key) select i.Value).FirstOrDefault();
        var controlledSummons = GetControlledSummons(levelState);
        foreach (var controlledSummon in controlledSummons)
            controlledSummon.CurrentState = CharacterState.Attacking;

        controlledSummonIds.Clear();
        if (fleeTarget != null)
            gameLogic.MoveCharacter(Enemy, fleeTarget.Position);
    }

    private void RecruitHostileSummons(LevelController levelState)
    {
        var hostiles = levelState.QuadTree.SearchCharacters(Enemy.VisionRectangle).Where(c => !c.IsFriendly).ToList();
        foreach (var hostileSummon in hostiles)
        {
            if (controlledSummonIds.Contains(hostileSummon.Id) || hostileSummon.Id == Enemy.Id) continue;

            controlledSummonIds.Add(hostileSummon.Id);
            hostileSummon.CurrentState = CharacterState.ArchEnemyControl;

            if (controlledSummonIds.Count >= UnitThreshold)
                break;
        }
    }

    private void CreateAndAddHostileSummon(LevelController levelState)
    {
        var element = GetElement(levelState);
        var hostile = CreateHostile(element);
        levelState.AddHostileSummon(hostile);
        isAbleToSummon = false;
        summonCapacity--;
    }

    private SummonModel CreateHostile(ElementType element)
    {
        SummonModel hostileSummon;
        switch (element)
        {
            case ElementType.Fire:
                hostileSummon = new PaladinController(CameraController.TileCenterToWorld(Enemy.Position.ToGrid()), Enemy.Skills[element])
                {
                    CurrentState = CharacterState.ArchEnemyControl
                };
                return hostileSummon;
            case ElementType.Lightning:
                hostileSummon = new PaladinController(CameraController.TileCenterToWorld(Enemy.Position.ToGrid()), Enemy.Skills[element])
                {
                    CurrentState = CharacterState.ArchEnemyControl
                };
                return hostileSummon;
            default:
                throw new ArgumentOutOfRangeException(nameof(element), element, $"Unhandled element type: {element}");
        }
    }

    private ElementType GetElement(LevelController levelState)
    {
        var controlledSummons = GetControlledSummons(levelState);
        var elementCount = new Dictionary<ElementType, int>()
        {
            { ElementType.Fire, 0},
            { ElementType.Lightning, 0},
            { ElementType.Ghost, 0},
            { ElementType.Magic, 0},
        };
        foreach (var controlledSummon in controlledSummons)
            elementCount[controlledSummon.Element]++;
        return elementCount.MinBy(i => i.Value).Key;
    }

    private bool IsTargetInRange(LevelController levelState)
    {
        var charactersInRange = GetGroupTargets(levelState);
        return charactersInRange.Count != 0 || GetControlledSummons(levelState)
            .Exists(summon => levelState.AttackerToTarget.ContainsKey(summon.Id)) || levelState.AttackerToTarget.ContainsKey(Enemy.Id);
    }

    private void UpdateControlledSummonList(LevelController levelState)
    {
        controlledSummonIds.RemoveAll(hostile => !levelState.HostileSummons.ContainsKey(hostile));
    }

    private List<CharacterController> GetGroupTargets(LevelController levelState)
    {
        var targets = levelState.QuadTree.SearchCharacters(Enemy.VisionRectangle).Where(character => character.IsFriendly).ToList();
        foreach (var controlledSummon in GetControlledSummons(levelState))
            targets.AddRange(levelState.QuadTree.SearchCharacters(controlledSummon.VisionRectangle)
                .Where(character => character.IsFriendly && !targets.Contains(character)));
        return targets;
    }

    private List<CharacterController> GetControlledSummons(LevelController levelState)
    {
        var charList = new List<CharacterController>();
        foreach (var controlledSummonId in controlledSummonIds)
        {
            var controlledSummon = levelState.GetSummonWithId(controlledSummonId);
            if (controlledSummon != null)
                charList.Add(controlledSummon);
        }
        return charList;
    }

    private bool MemberInCombat(LevelController levelState) =>
        levelState.AttackerToTarget.ContainsKey(Enemy.Id) || GetControlledSummons(levelState).Any(summon => levelState.AttackerToTarget.ContainsKey(summon.Id));

    public void Initialize(CharacterController enemy) { Enemy = (ArchenemyController)enemy; }
    private void WalkTimerElapsed(object sender, ElapsedEventArgs e) { isAbleToWalk = true; }
    private void SummonTimerElapsed(object sender, ElapsedEventArgs e) { isAbleToSummon = true; }
    private void BearTrapTimerElapsed(object sender, ElapsedEventArgs e) { isAbleToThrowTrap = true; }
}