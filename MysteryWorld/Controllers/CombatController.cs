#nullable enable
using System.Collections.Generic;
using MysteryWorld.Models;
using MysteryWorld.Models.Enums;
using MysteryWorld.Models.Interfaces;
using MysteryWorld.Views;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace MysteryWorld.Controllers;

public sealed class CombatController
{
    private readonly LevelController LevelState;
    private readonly IPathfinder Pathfinder;

    private Dictionary<string, string> AttackerToTarget => LevelState.AttackerToTarget;
    private Dictionary<string, HashSet<string>> Attackers => LevelState.Attackers;
    private readonly EventController EventDispatcher;

    public CombatController(LevelController levelState, IPathfinder pathfinder, EventController eventDispatcher)
    {
        LevelState = levelState;
        Pathfinder = pathfinder;
        EventDispatcher = eventDispatcher;
    }

    public void Update()
    {
        foreach (var combatPair in AttackerToTarget)
        {
            var attacker = LevelState.GetCharacterWithId(combatPair.Key);
            var target = LevelState.GetCharacterWithId(combatPair.Value);

            if (attacker == null || target == null) continue;

            if (!IsInRange(attacker, target))
            {
                if (attacker.movementState == MovementState.Idle && attacker.CurrentState is CharacterState.Attacking or CharacterState.PlayerControl or CharacterState.ArchEnemyControl)
                    MoveAttackerToTarget(attacker, target.Position);
                continue;
            }

            attacker.Timer += 1;
            attacker.StopMovement();
            if (attacker.Timer % attacker.Delay != 0) continue;

            switch (attacker.CombatType)
            {
                case CombatType.Melee:
                    MeleeAttack(attacker, target);
                    break;
                case CombatType.AoE:
                    PerformMeleeAttack(attacker, target);
                    break;
                case CombatType.Ranged:
                    RangedAttack(attacker, target);
                    break;
                case CombatType.SelfDamage:
                    PerformSelfDamageAttack(attacker, target);
                    break;
            }
        }
    }

    private void MeleeAttack(CharacterController attacker, CharacterController target)
    {
        DealsDamage(attacker, target);
    }

    private void PerformMeleeAttack(CharacterController attacker, GameObjectView target)
    {
        var charactersInRange = LevelState.QuadTree.SearchCharacters(new Rect(
            new Vector2(
                target.Position.X - attacker.Range / 2,
                target.Position.Y - attacker.Range / 2),
            new Vector2(attacker.Range,
                attacker.Range)));

        foreach (var character in charactersInRange)
            if (attacker.IsFriendly != character.IsFriendly)
                DealsDamage(attacker, character);
    }

    private void PerformSelfDamageAttack(CharacterController attacker, GameObjectView target)
    {
        var charactersInRange = LevelState.QuadTree.SearchCharacters(new Rect(
            new Vector2(target.Position.X - attacker.Range, target.Position.Y - attacker.Range),
            new Vector2(attacker.Range * 2, attacker.Range * 2)));

        foreach (var character in charactersInRange)
            DealsDamage(attacker, character);
    }

    private void RangedAttack(CharacterController attacker, GameObjectView target)
    {
        ShootProjectile(attacker, target);
        attacker.Timer = 1;
    }

    private void ShootProjectile(CharacterController attacker, GameObjectView target)
    {
        var direction = target.Position - attacker.Position;
        direction.Normalize();
        var start = attacker.Position;
        var destination = attacker.Position + attacker.Range * 2 * direction;
        var projectile = CreateProjectile(attacker, start, destination);
        if (projectile != null)
            LevelState.AddProjectile(projectile);
    }

    public void SelectTarget(string attackerId, CharacterController target)
    {
        RemoveTarget(attackerId);

        AttackerToTarget[attackerId] = target.Id;
        if (Attackers.ContainsKey(target.Id))
        {
            Attackers[target.Id].Add(attackerId);
            return;
        }

        var attackerIds = new HashSet<string>() { attackerId };
        Attackers[target.Id] = attackerIds;
        if (!target.IsFriendly)
            target.Selected = true;
    }

    public void SetTargetWithId(string attackerId, string targetId)
    {
        var target = LevelState.GetCharacterWithId(targetId);
        if (target != null)
            SelectTarget(attackerId, target);
    }

    public void RemoveTarget(string attackerId)
    {
        if (!AttackerToTarget.ContainsKey(attackerId)) return;
        var currentTargetId = AttackerToTarget[attackerId];
        if (!Attackers.ContainsKey(currentTargetId)) return;

        Attackers[currentTargetId].Remove(attackerId);
        if (Attackers[currentTargetId].Count == 0)
        {
            Attackers.Remove(currentTargetId);
            var enemy = LevelState.GetEnemyWithId(currentTargetId);
            if (enemy != null) enemy.Selected = false;
        }
        AttackerToTarget.Remove(attackerId);
    }

    public void MoveAttackerToTarget(CharacterController attacker, Vector2 targetPos)
    {
        var path = Pathfinder.CalculatePath(attacker.Position.ToGrid(), targetPos.ToGrid());
        attacker.SetPath(path);
    }

    private static bool IsInRange(CharacterController attacker, GameObjectView target) =>
        (attacker.Position - target.Position).Length() < attacker.Range + 32;

    private void DealsDamage(CharacterController attacker, CharacterController target)
    {
        attacker.ChangeAnimation(Animations.AttackAnimation);
        attacker.Timer = 1;
        if (target is SummonerController)
            if (LevelState.IsInTechnicalState) return;
        target.GetHit((int)(attacker.Damage + attacker.DamageUp.Active * attacker.DamageUp.Strength), attacker.Element.ElementalEffectiveness(target.Element));
    }

    private static ProjectileView? CreateProjectile(CharacterController attacker, Vector2 start, Vector2 destination)
    {
        attacker.ChangeAnimation(Animations.AttackAnimation);
        switch (attacker)
        {
            default:
                return null;
        }
    }
}