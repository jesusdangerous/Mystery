using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MysteryWorld.Models.Enums;
using MysteryWorld.Controllers;

namespace MysteryWorld.Views
{
    public abstract class ProjectileView : GameObjectView
    {
        private const float ProjectileLayer = 0.5f;
        private const float F2 = 2f;
        private const float F3 = 3f;

        public ProjectileType Type { get; protected set; }
        public Vector2 Destination { get; protected set; }
        public float Velocity { get; protected set; }
        public int Damage { get; protected set; }

        public ProjectileEffect Effect { get; protected set; }
        public float Angle { get; protected set; }
        public bool IsFriendly { get; private set; }
        public string CharacterId { get; private set; }
        public ElementType Element { get; set; }

        protected ProjectileView(Vector2 position, Vector2 destination, float velocity, bool isFriendly, string characterId, int damage, ElementType element) : base(position)
        {
            Destination = destination;
            Velocity = velocity;
            Angle = GetAngle();
            CurrentSpriteId = SpriteId;
            IsFriendly = isFriendly;
            CharacterId = characterId;
            Damage = damage;
            Element = element;
            LayerDepth = ProjectileLayer;
        }
        public new Rectangle Hitbox => new(
            (int)(Position.X - GameController.ScaledPixelSize / F3) + 1,
            (int)(Position.Y - GameController.ScaledPixelSize / F3) + 1,
            (int)(GameController.ScaledPixelSize * (F2 / F3) - 1),
            (int)(GameController.ScaledPixelSize * (F2 / F3) - 1));

        public override void Update(float deltaTime)
        {
            UpdateAnimation(deltaTime);
            var directionalVector = Destination - Position;
            directionalVector.Normalize();
            var moveDistance = deltaTime * Velocity * directionalVector;
            if (Vector2.Distance(Position, Destination) > moveDistance.Length())
            {
                Position += moveDistance;
                return;
            }
            State = InstanceState.LimitReached;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (GameController.DebugMode)
                spriteBatch.DrawRectangle(Hitbox, 1, Color.Red);

            spriteBatch.Draw(AssetController.SpriteSheet,Position, AssetController.GetRectangle(SpriteId + currentFrame),
                Color.White, Angle, GameController.Origin, GameController.Scale, SpriteEffects.None, LayerDepth);
        }

        private float GetAngle()
        {
            var v1 = -Vector2.UnitX;
            var v2 = Destination - Position;
            v1.Normalize();
            v2.Normalize();
            return (float)Math.Atan2(v1.X * v2.Y - v1.Y * v2.X, v1.X * v2.X + v1.Y * v2.Y);
        }
    }
}