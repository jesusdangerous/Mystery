using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MysteryWorld.Models.Interfaces;

public interface IPathfinder
{
    public List<Vector2> CalculatePath(Vector2 start, Vector2 goal);
}