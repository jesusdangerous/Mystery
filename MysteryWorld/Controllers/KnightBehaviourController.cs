#nullable enable
using System;
using System.Linq;
using System.Timers;
using System.Security.Cryptography;
using Microsoft.Xna.Framework;
using MysteryWorld.Models;
using MysteryWorld.Models.Enums;
using MysteryWorld.Models.Interfaces;

namespace MysteryWorld.Controllers;

public class KnightBehaviourController : EnemyModel, IEnemyBehaviour
{
    private const int TimerInterval = 5000;

    private CharacterState EnemyState => Enemy.CurrentState;

    private readonly Timer Timer;
    private bool isCanWalk;

    private void TimerElapsed(object? sender, ElapsedEventArgs e) { isCanWalk = true; }

    public KnightBehaviourController(CharacterController paladin)
    {
        Enemy = paladin;
        Timer = new Timer(TimerInterval);
        Timer.Elapsed += TimerElapsed;
        Timer.AutoReset = true;
        Timer.Start();
        isCanWalk = false;
    }

    public void UpdateState(LevelController levelState, GameModel gameLogic)
    {
        switch (EnemyState)
        {
            case CharacterState.Idle:
                break;
            case CharacterState.Patrolling:
                Patrolling(levelState, gameLogic);
                break;
            case CharacterState.Attacking:
                Attacking(levelState);
                break;
            case CharacterState.Fleeing:
                Fleeing();
                break;
            case CharacterState.ArchEnemyControl:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static void Fleeing()
    {
    }

    private void Attacking(LevelController levelState)
    {
        if (!levelState.AttackerToTarget.ContainsKey(Enemy.Id))
            Enemy.CurrentState = CharacterState.Patrolling;
    }

    private void Patrolling(LevelController levelState, GameModel gameLogic)
    {
        if (levelState.AttackerToTarget.ContainsKey(Enemy.Id))
        {
            Enemy.CurrentState = CharacterState.Attacking;
            Taunt(levelState, gameLogic);
            return;
        }

        var target = FindTargetInVision(levelState);
        if (target != null)
        {
            Enemy.CurrentState = CharacterState.Attacking;
            gameLogic.AttackCharacter(Enemy, target);
            Taunt(levelState, gameLogic);
            return;
        }

        if (SetPathToFriend(levelState, gameLogic)) return;
        if (isCanWalk) SetRandomPath(levelState, gameLogic);
    }

    private CharacterController? FindTargetInVision(LevelController levelState) =>
        levelState.QuadTree.SearchCharacters(Enemy.VisionRectangle).FirstOrDefault(c => c.IsFriendly);

    private void SetRandomPath(LevelController levelState, GameModel gameLogic)
    {
        if (Enemy.movementState == MovementState.Moving) return;

        var gridCoordinates = Enemy.Position.ToGrid();
        var neighbors = levelState.GameMap.PassableNeighbors(gridCoordinates).ToList();

        if (neighbors.Count == 0) return;

        var randomInt = RandomNumberGenerator.GetInt32(0, neighbors.Count);

        gameLogic.MoveCharacter(Enemy, CameraController.TileCenterToWorld(neighbors[randomInt]));
        isCanWalk = false;
    }


    private bool SetPathToFriend(LevelController levelState, GameModel gameLogic)
    {
        if (Enemy.movementState == MovementState.Moving) return true;
        if (isCanWalk) return SetPathToFriendHelper(levelState, gameLogic);
        return false;
    }

    private bool SetPathToFriendHelper(LevelController levelState, GameModel gameLogic)
    {
        if (FriendIsAttacked(levelState, gameLogic)) return true;

        var friendsInRange = levelState.QuadTree
            .SearchCharacters(Enemy.VisionRectangle)
            .Where(i => !i.IsFriendly)
            .ToList();

        if (friendsInRange.Count != 0)
            foreach (var friend in friendsInRange)
            {
                if (friend is PaladinController) continue;
                gameLogic.MoveCharacter(Enemy, friend.Position);
                isCanWalk = false;
                return true;
            }
        return false;
    }

    private void Taunt(LevelController levelState, GameModel gameLogic)
    {
        var charactersInRange = levelState.QuadTree.SearchCharacters(Enemy.VisionRectangle).Where(c => c.IsFriendly).ToList();
        if (charactersInRange.Count > 0)
            gameLogic.AttackCharacter(charactersInRange[0], Enemy);
    }

    private bool FriendIsAttacked(LevelController levelState, GameModel gameLogic)
    {
        var friendsInRange = levelState.QuadTree.SearchCharacters(new Rect(
            new Vector2(Enemy.Position.X - 2 * Enemy.Vision, Enemy.Position.Y - 2 * Enemy.Vision),
            new Vector2(4 * Enemy.Vision, 4 * Enemy.Vision))).Where(c => !c.IsFriendly).ToList();
        foreach (var friend in friendsInRange)
            if (friend.CurrentState == CharacterState.Attacking || friend.CurrentState == CharacterState.Fleeing)
            {
                gameLogic.MoveCharacter(Enemy, friend.Position);
                isCanWalk = false;
                return true;
            }
        return false;
    }
}