using MysteryWorld.Models;
using MysteryWorld.Models.Interfaces;

namespace MysteryWorld.Controllers;

public sealed class EnemyBehaviourController
{
    private int Performeter;
    private int NextStart = 1;
    private const int Frequency = 5;

    public EnemyBehaviourController(LevelController levelState)
    {
        levelState.ArchEnemy?.Initialize(levelState.ArchEnemy);

        foreach (var enemy in levelState.HostileSummons)
            if (enemy.Value is IEnemyBehaviour ai)
                ai.Initialize(enemy.Value);
    }

    public void Update(LevelController levelState, GameModel gameLogic)
    {
        levelState.ArchEnemy?.UpdateState(levelState, gameLogic);
        foreach (var enemy in levelState.HostileSummons)
        {
            Performeter = (Performeter + 1) % Frequency;
            if (Performeter != 0) continue;
            if (enemy.Value is IEnemyBehaviour ai)
                ai.UpdateState(levelState, gameLogic);
        }
        Performeter = NextStart;
        NextStart = (NextStart + 1) % Frequency;
    }
}