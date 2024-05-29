using Microsoft.Xna.Framework;
using MysteryWorld.Models.Enums;
using MysteryWorld.Views;

namespace MysteryWorld.Models;

internal sealed class FireballModel : ProjectileView
{
    private const int Sprite = 2752;

    private const float F4 = 4f;

    public FireballModel(Vector2 position, Vector2 destination, bool isFriendly, string characterId, int damage, ElementType element)
        : base(position, destination, F4 * GameController.ScaledPixelSize, isFriendly, characterId, damage, element)
    {
        SpriteId = Sprite;
        Type = ProjectileType.Fireball;
        Damage = damage;
        Effect = ProjectileEffect.None;
    }
}