using MysteryWorld.Controllers;

namespace MysteryWorld.Models.Interfaces;

public interface IEnemyBehaviour
{
    public void UpdateState(LevelController levelState, GameModel gameLogic);
    public void Initialize(CharacterController enemy);
}