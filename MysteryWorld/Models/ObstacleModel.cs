using Microsoft.Xna.Framework;
using MysteryWorld.Models.Enums;
using MysteryWorld.Views;

namespace MysteryWorld.Models;

public class ObstacleModel : ProjectileView
{
    private const int Sprite = 2792;

    public ObstacleModel(Vector2 position, Vector2 destination, bool isFriendly, string characterId, int damage, ElementType element) : base(position, destination, 0f, isFriendly, characterId, damage, element)
    {
        SpriteId = Sprite;
        Type = ProjectileType.BearTrap;
        Angle = 0f;
        Effect = ProjectileEffect.None;
    }
}