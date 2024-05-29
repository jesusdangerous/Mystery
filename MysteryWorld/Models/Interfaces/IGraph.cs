using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MysteryWorld.Models.Interfaces;

public interface IGraph
{
    public bool Passable(Vector2 id);
    double Cost(Vector2 a, Vector2 b);
    IEnumerable<Vector2> PassableNeighbors(Vector2 id);
}