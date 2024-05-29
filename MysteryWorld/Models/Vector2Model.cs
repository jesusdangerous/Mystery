using Microsoft.Xna.Framework;

namespace MysteryWorld.Models;

public static class Vector2Model
{
    public static Vector2 ToGrid(this Vector2 vec) =>
        new((int)(vec.X / GameController.ScaledPixelSize), (int)(vec.Y / GameController.ScaledPixelSize));
}