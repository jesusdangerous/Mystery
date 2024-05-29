using Microsoft.Xna.Framework;
using MysteryWorld.Controllers;

namespace MysteryWorld.Models;

public abstract class SummonModel : CharacterController
{
    protected SummonModel(Vector2 position, int level, bool friendly) : base(position, level, friendly)
    {
    }
}