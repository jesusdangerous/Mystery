using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MysteryWorld.Models.Enums;
using MysteryWorld.Controllers;

namespace MysteryWorld.Views;

public abstract class GameObjectView
{
    protected const int TotalFrames = 7;
    private const int FancyAnimationOffset = 8;
    private const int WalkAnimationOffset = 16;
    private const int AttackAnimationOffset = 24;

    protected const float TimeToNextFrame = 0.105f;
    private const float F2 = 2f;

    public Vector2 Position { get; set; }
    public int SpriteId { get; protected set; }
    public int CurrentSpriteId { get; protected set; }
    public InstanceState State { get; set; } = InstanceState.Pending;
    public string Id { get; protected set; } = Guid.NewGuid().ToString();
    public float LayerDepth { get; protected set; } = 0.1f;

    public bool isNotAnimated;
    public int currentFrame;
    public Animations currentAnimation;
    protected float animationTimer;

    public Rectangle Hitbox => new((int)(Position.X - GameController.ScaledPixelSize / F2) + 1, (int)(Position.Y - GameController.ScaledPixelSize / F2) + 1,
            (int)GameController.ScaledPixelSize - 1, (int)GameController.ScaledPixelSize - 1);

    protected GameObjectView(Vector2 position)
    {
        Position = position;
        CurrentSpriteId = SpriteId;
    }

    public void ChangeAnimation(Animations animation)
    {
        if (isNotAnimated || animation == currentAnimation) return;

        switch (animation)
        {
            case Animations.IdleAnimation:
                CurrentSpriteId = SpriteId;
                currentAnimation = Animations.IdleAnimation;
                break;
            case Animations.FancyIdleAnimation:
                CurrentSpriteId = SpriteId + FancyAnimationOffset;
                currentAnimation = Animations.FancyIdleAnimation;
                break;
            case Animations.WalkingAnimation:
                CurrentSpriteId = SpriteId + WalkAnimationOffset;
                currentAnimation = Animations.WalkingAnimation;
                break;
            case Animations.AttackAnimation:
                CurrentSpriteId = SpriteId + AttackAnimationOffset;
                currentFrame = 0;
                currentAnimation = Animations.AttackAnimation;
                break;
        }
    }

    protected void UpdateAnimation(float deltaTime)
    {
        if (isNotAnimated) return;

        if (animationTimer > TimeToNextFrame)
        {
            animationTimer = 0f;
            currentFrame += 1;

            if (currentFrame > TotalFrames)
                currentFrame = 0;
        }
        animationTimer += deltaTime;
    }

    public virtual void Update(float deltaTime)
    {
        UpdateAnimation(deltaTime);
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        if (GameController.DebugMode)
            spriteBatch.DrawRectangle(Hitbox, 1, Color.Red);

        spriteBatch.Draw(AssetController.SpriteSheet, Position, AssetController.GetRectangle(CurrentSpriteId + currentFrame),
        Color.White, 0f, GameController.Origin, GameController.Scale, SpriteEffects.None, LayerDepth);
    }
}