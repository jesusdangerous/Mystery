using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using MysteryWorld.Models;
using MysteryWorld.Models.Enums;
using MysteryWorld.Views;

namespace MysteryWorld.Controllers;

public abstract class CharacterController : GameObjectView
{
    private const int DamageUpIconId = 3136;
    private const int HealUpIconId = 3137;
    private const int SpeedUpIconId = 3138;

    private const int SoulChance = 30;
    private const int HealthPotionChance = 18;
    private const int NothingChance = 40;

    private const float F1 = 0.1f;
    private const float F7 = 0.7f;
    private const float F15 = 1.5f;
    private const float F2 = 2f;
    private const float F4 = 4f;
    private const float MaxLevel = 5f;

    private const float F10 = 10f;

    public int MaxLifePoints { get; set; }
    public int CurrentLifePoints { get; set; }
    public int Damage { get; protected set; }
    public int Level { get; set; } = 1;
    public int DrawLevel { get; protected set; }
    public bool Selected { get; set; }
    public Vector2 Destination { get; set; }
    public ElementType Element { get; protected set; }
    public List<(DropAbleLoot type, int chance)> PossibleLoot { get; protected set; } = new() { (DropAbleLoot.Soul, SoulChance), (DropAbleLoot.HealthPotion, HealthPotionChance), (DropAbleLoot.Nothing, NothingChance) };
    public float Velocity { get; set; } = F4 * GameController.ScaledPixelSize;
    public Vector2 Direction { get; set; }
    public List<Vector2> Path { get; private set; } = new();
    private bool isFlipped;

    public MovementState movementState = MovementState.Idle;

    private readonly Random randomInt = new();

    private Color color = Color.White;

    private int DelayCounter;

    public bool IsFriendly { get; protected set; }

    public float Range { get; protected set; } = F4 * GameController.ScaledPixelSize;
    public float Vision { get; protected set; } = F10 * GameController.ScaledPixelSize;

    public Rectangle VisionRectangle => new((int)(Position.X - Vision / 2), (int)(Position.Y - Vision / 2), (int)Vision, (int)Vision);
    public CombatType CombatType { get; protected set; }
    public int Timer { get; set; }
    public int Delay { get; protected set; } = 120;

    public TempModel HealUp { get; set; } = new();
    public TempModel SpeedUp { get; set; } = new();
    public TempModel DamageUp { get; set; } = new();

    public CharacterState CurrentState { get; set; } = CharacterState.PlayerControl;
    private float fancyAnimationTimer;
    private int fancyAnimationLimit;


    protected CharacterController(Vector2 position) : base(position)
    {
        Destination = position;
        Direction = Vector2.Zero;
        CurrentSpriteId = SpriteId;
        UpdateLevel(Level);
        LayerDepth = F7;
    }

    protected CharacterController(Vector2 position, int level, bool friendly = false) : base(position)
    {
        Destination = position;
        Direction = Vector2.Zero;
        CurrentSpriteId = SpriteId;
        IsFriendly = friendly;
        UpdateLevel(level);
        LayerDepth = F7;
        fancyAnimationLimit = (randomInt.Next() % 5 + 1) * 5;
    }

    private void UpdateLevel(int level)
    {
        if (IsFriendly)
        {
            DrawLevel = (int)Math.Floor(level / MaxLevel * F2);
            return;
        }
        DrawLevel = level - 1;
    }

    private new void UpdateAnimation(float deltaTime)
    {
        if (isNotAnimated) return;

        if (animationTimer > TimeToNextFrame)
        {
            animationTimer = 0f;
            currentFrame += 1;

            if (currentFrame > TotalFrames)
            {
                currentFrame = 0;
                if (fancyAnimationTimer > fancyAnimationLimit && CurrentState != CharacterState.Attacking && currentAnimation == Animations.IdleAnimation)
                {
                    ChangeAnimation(Animations.FancyIdleAnimation);
                    fancyAnimationTimer = 0f;
                    fancyAnimationLimit = (randomInt.Next() % 5 + 1) * 5;
                }
                else ChangeAnimation(Animations.IdleAnimation);
            }
        }
        animationTimer += deltaTime;
        fancyAnimationTimer += deltaTime;
    }
    public void SetPath(List<Vector2> path)
    {
        Path = path;
        movementState = MovementState.Moving;

        if (path.Count <= 1) return;

        Path.RemoveAt(Path.Count - 1);
        Destination = Path[^1];
    }

    public void SetPosition(Vector2 position)
    {
        Path.Clear();
        Direction = Vector2.Zero;
        Position = position;
        Destination = position;
        movementState = MovementState.Idle;
    }

    public void StopMovement()
    {
        SetPosition(Position);
    }

    private void CalculateDirection()
    {
        var directionalVector = Destination - Position;
        directionalVector.Normalize();
        Direction = directionalVector;
    }

    public override void Update(float deltaTime)
    {
        Heal();
        HealUp.Update(deltaTime);
        SpeedUp.Update(deltaTime);
        DamageUp.Update(deltaTime);
        UpdateAnimation(deltaTime);

        switch (movementState)
        {
            case MovementState.Idle:
                break;
            case MovementState.Moving:
                Move(deltaTime);
                break;
        }
    }

    private void Move(float deltaTime)
    {
        if (Path.Count > 0)
        {
            CalculateDirection();
            var moveDistance = deltaTime * (Velocity + SpeedUp.Active * SpeedUp.Strength * GameController.ScaledPixelSize) *
                               Direction;
            if (Vector2.Distance(Position, Destination) > moveDistance.Length())
            {
                ChangeAnimation(Animations.WalkingAnimation);
                isFlipped = Direction.X >= 0;
                Position += moveDistance;
                return;
            }

            Path.RemoveAt(Path.Count - 1);
            if (Path.Count == 0)
            {
                Position = Destination;
                Direction = Vector2.Zero;
                ChangeAnimation(Animations.IdleAnimation);
                movementState = MovementState.Idle;
                return;
            }
            Destination = Path[^1];
        }
    }

    private SpriteEffects GetSpriteEffect() =>
        isFlipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (GameController.DebugMode)
        {
            spriteBatch.DrawRectangle(Hitbox, 1, Color.Red);
            foreach (Vector2 vector in Path)
            {
                var hitbox = new Rectangle((int)(vector.X - GameController.ScaledPixelSize / F2),
                  (int)(vector.Y - GameController.ScaledPixelSize / F2),
                  (int)GameController.ScaledPixelSize,
                  (int)GameController.ScaledPixelSize);

                spriteBatch.DrawRectangle(hitbox, 4, Color.PowderBlue);
            }
            DrawCurrentEnemyAiState(spriteBatch, Color.LightBlue);
            spriteBatch.DrawRectangle(new Rectangle((int)(Position.X - Vision / F2), (int)(Position.Y - Vision / F2), (int)Vision, (int)Vision), 2, Color.Green);
            spriteBatch.DrawRectangle(new Rectangle((int)(Position.X - Range / 1f), (int)(Position.Y - Range / 1f), (int)(2 * Range), (int)(2 * Range)), 2, Color.IndianRed);
        }
        spriteBatch.Draw(AssetController.SpriteSheet, Position,
            AssetController.GetRectangle((Selected ? CurrentSpriteId + 32 + DrawLevel * 64 : CurrentSpriteId + DrawLevel * 64) + currentFrame),
            color, 0f, GameController.Origin, GameController.Scale, GetSpriteEffect(), LayerDepth);
        DrawOverHeadStats(spriteBatch, Color.DarkRed, Color.DarkGreen);
        DrawBuffs(spriteBatch);
        if (color == Color.Red)
        {
            if (DelayCounter > 20)
            {
                color = Color.White;
                DelayCounter = 0;
            }
            else DelayCounter += 1;
        }
    }

    private float GetLifeStatus() =>
        (float)CurrentLifePoints / MaxLifePoints;

    private void DrawBuffs(SpriteBatch spriteBatch)
    {
        if (DamageUp.Active == 1)
            spriteBatch.Draw(AssetController.SpriteSheet, Position + new Vector2(-25, -50), AssetController.GetRectangle(DamageUpIconId),
                Color.White, 0f, GameController.Origin, Vector2.One * F15, SpriteEffects.None, LayerDepth);

        if (HealUp.Active == 1)
            spriteBatch.Draw(AssetController.SpriteSheet, Position + new Vector2(0, -50), AssetController.GetRectangle(HealUpIconId),
                Color.White, 0f, GameController.Origin, Vector2.One * F15, SpriteEffects.None, LayerDepth);

        if (SpeedUp.Active == 1)
            spriteBatch.Draw(AssetController.SpriteSheet, Position + new Vector2(25, -50), AssetController.GetRectangle(SpeedUpIconId),
                Color.White, 0f, GameController.Origin, Vector2.One * F15, SpriteEffects.None, LayerDepth);
    }

    private void DrawOverHeadStats(SpriteBatch spriteBatch, Color color1, Color color2)
    {
        var life = GetLifeStatus();
        var position = new Vector2(Position.X - 30, Position.Y + 35);
        var rect = new Rectangle((int)(Position.X - 30), (int)(Position.Y + 35), 50, 10);
        spriteBatch.Draw(AssetController.hudPanelBar, position, rect, color1, 0f, Vector2.Zero, Vector2.One,
            SpriteEffects.None, LayerDepth);

        spriteBatch.Draw(AssetController.hudPanelBar, position, new Rectangle((int)(Position.X - 30), (int)(Position.Y + 35), (int)(life * 50), 10),
            color2, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, LayerDepth - F1);
    }

    private void DrawCurrentEnemyAiState(SpriteBatch spriteBatch, Color color1)
    {
        var currentStateString = CurrentState.ToString();
        var pos = new Vector2(Position.X - 30, Position.Y - 50);
        spriteBatch.DrawString(AssetController.hudFont, currentStateString, pos, color1, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
    }

    public void GetHit(float damage, float scaling)
    {
        CurrentLifePoints -= (int)(damage * scaling);
        color = Color.Red;
    }

    public ItemModel DropItem()
    {
        var possibleSum = PossibleLoot.Sum(loot => loot.chance);
        if (possibleSum != 100) return null;

        var rand = randomInt.Next();
        var curSum = 0;
        foreach (var loot in PossibleLoot)
        {
            curSum += loot.chance;
            if (rand % 100 >= curSum) continue;

            var lootPos = CameraController.TileCenterToWorld(Position.ToGrid());
            ItemModel outItem = loot.type switch
            {
                DropAbleLoot.HealthPotion => new HealthPotionModel(lootPos),
                DropAbleLoot.Nothing => null,
                _ => null
            };
            return outItem;
        }
        return null;
    }

    private void Heal()
    {
        CurrentLifePoints = (int)Math.Clamp(CurrentLifePoints + HealUp.Active * HealUp.Strength, -1, MaxLifePoints);
    }

    protected int AddIntScaling(int baseVal, float increment) =>
        (int)(baseVal + increment * (Level - 1));

    protected float AddFloatScaling(float baseVal, float increment) =>
        baseVal + increment * (Level - 1);

    public void SetColor(Color color)
    {
        this.color = color;
    }

    public void UseProjectileEffect(ProjectileEffect projectileEffect, int healingStrength, int speedStrength)
    {
        switch (projectileEffect)
        {
            case ProjectileEffect.None:
                break;
            case ProjectileEffect.Healing:
                HealUp = TempModel.CreateTemporaryEffect(10, healingStrength);
                break;
            case ProjectileEffect.Speed:
                SpeedUp = TempModel.CreateTemporaryEffect(10, speedStrength);
                break;
        }
    }
}