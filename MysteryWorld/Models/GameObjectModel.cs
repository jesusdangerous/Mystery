using MysteryWorld.Views;

namespace MysteryWorld.Models;

public static class GameObjectModel
{
    public static bool IntersectsWith(this GameObjectView gameObject1, GameObjectView gameObject2) =>
        gameObject1.Hitbox.Intersects(gameObject2.Hitbox);
}