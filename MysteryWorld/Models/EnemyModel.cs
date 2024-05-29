using MysteryWorld.Controllers;

namespace MysteryWorld.Models;

public abstract class EnemyModel
{
    protected CharacterController Enemy;

    public void Initialize(CharacterController enemy)
    {
        Enemy = enemy;
    }
}