using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MysteryWorld.Models.Enums;
using MysteryWorld.Models;

namespace MysteryWorld.Controllers;

public sealed class SummonerController : MainCharacterModel
{
    public int FireballDamage { get; private set; } = 50;
    public int HealingStrength { get; private set; } = 1;
    public int SpeedStrength { get; private set; } = 1;

    public float FireBallCoolDown { get; set; }
    public bool EnoughManaFireBall { get; set; } = true;
    private const int FireBallCost = 20;
    public const int CoolDownLimitFire = 1;

    public float HealingSpellCoolDown { get; set; }
    public bool EnoughHealthHealing { get; set; } = true;
    public const int CoolDownLimitHeal = 2;

    public float SpeedSpellCoolDown { get; set; }
    public bool EnoughManaSpeed { get; set; } = true;
    private const int SpeedSpellCost = 50;
    public const int CoolDownLimitSpeed = 3;

    private const int Sprite = 64;
    private const int XpBase = 100;
    private const int ManaBase = 100;
    private const int ManaScale = 5;
    private const int HpBase = 666;
    private const int HpScale = 30;
    private const int DamageBase = 45;
    private const int DamageScale = 5;
    private const int DelayBase = 110;
    private const int FireBallDamageBase = 50;
    private const int FireBallDamageScale = 5;

    private const float GridSpeedBase = 5f;
    private const float GridRange = 1f;
    private const float GridVision = 7f;
    private const float SummonExtend = 3.5f;

    private const int I2 = 2;

    private const float SkillPassiveManaRegeneration = 0.015f;
    private const float F05 = 0.5f;
    private const float F08 = 0.8f;
    private const float BaseManaRegeneration = 0.08f;

    public int Souls { get; set; }
    public int SkillPoints { get; set; }
    public SummonType? SelectedSummonType { get; set; }
    public float ActualMana { get; set; }
    public float SummonRange { get; set; }
    public float HealHpCost { get; set; } = 0.18f;

    public SummonerController(Vector2 position) : base(position)
    {
        Skills = new Dictionary<ElementType, int>
        {
            { ElementType.Fire, 1 },
            { ElementType.Ghost, 1 },
            { ElementType.Lightning, 1 },
            { ElementType.Magic, 1 }
        };

        UpdateScalingStats();
        SpriteId = Sprite;
        CurrentSpriteId = SpriteId;
        AccumulatedXp = 0;
        CurrentMana = MaxMana;
        ActualMana = MaxMana;
        CurrentLifePoints = MaxLifePoints;
        Delay = DelayBase;
        Velocity = GridSpeedBase * GameController.ScaledPixelSize;
        Range = GridRange * GameController.ScaledPixelSize;
        Vision = GridVision * GameController.ScaledPixelSize;
        Element = ElementType.Neutral;
        Souls = 0;
        IsFriendly = true;
        SummonRange = SummonExtend;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        if (SelectedSummonType == null) return;
        spriteBatch.Draw(AssetController.SpriteSheet, Position + new Vector2(0, -GameController.ScaledPixelSize * F08),
            AssetController.GetRectangle((int)SelectedSummonType + currentFrame), Color.White, 0f, GameController.Origin,
            GameController.Scale * F05, SpriteEffects.None, 0f);
    }

    public bool CanSpawnCharacterInRange(Vector2 id, int range)
    {
        var rec = new Rectangle((int)Position.X - range, (int)Position.Y - range, I2 * range, I2 * range);
        return rec.Contains(id);
    }

    public bool IsCanSummon()
    {
        return SelectedSummonType switch
        {
            SummonType.MagicSeedling => Souls >= 1,
            _ => false
        };
    }

    public bool AddExp(int exp)
    {
        AccumulatedXp += exp;
        if (AccumulatedXp < MaxXp) return false;

        AccumulatedXp -= MaxXp;
        XpLevel++;
        SkillPoints++;
        UpdateScalingStats();
        return true;
    }

    public void UpdateSkills(Dictionary<ElementType, int> newSkills, int newSkillPoints)
    {
        Skills = newSkills;
        SkillPoints = newSkillPoints;
        UpdateScalingStats();
    }

    private void UpdateScalingStats()
    {
        MaxXp = McAddIntScaling(XpBase, 40 + 8 * XpLevel);
        MaxMana = McAddIntScaling(ManaBase, ManaScale) + (Skills[ElementType.Ghost] - 1) * ManaScale;
        MaxLifePoints = McAddIntScaling(HpBase, HpScale);
        CurrentLifePoints += HpScale;
        Damage = McAddIntScaling(DamageBase, DamageScale);
        FireballDamage = FireBallDamageBase + (Skills[ElementType.Fire] - 1) * FireBallDamageScale;
        SpeedStrength = Skills[ElementType.Lightning];
    }

    internal void RegenerateMana()
    {
        ActualMana = Math.Clamp(ActualMana + (BaseManaRegeneration + SkillPassiveManaRegeneration * Skills[ElementType.Magic]), 0, MaxMana);
        CurrentMana = (int)Math.Floor(ActualMana);
    }

    internal void CheckEnoughManaSpell()
    {
        EnoughManaFireBall = !(ActualMana < FireBallCost);
        EnoughManaSpeed = !(ActualMana < SpeedSpellCost);
        EnoughHealthHealing = CurrentLifePoints >= (int)(MaxLifePoints * HealHpCost);
    }
}