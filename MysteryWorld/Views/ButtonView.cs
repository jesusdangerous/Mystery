#nullable enable
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MysteryWorld.Controllers;
using MysteryWorld.Models;
using MysteryWorld.Models.Enums;
using MysteryWorld.Models.Interfaces;

namespace MysteryWorld.Views;

public sealed class ButtonView : MenuElementModel
{
    private const float HoverOpacity = 0.7f;
    private const float CenterFactor = 2f;
    private const int IconScaleFactor = 2;

    public event Action? OnClick;

    private readonly Texture2D buttonTexture;
    private readonly string? toolTipText;
    private readonly SpriteFont? font;
    private readonly int iconSpriteId;
    private readonly Vector2 iconPosition;

    private bool isHovered;
    private string? text;
    private Rectangle buttonRectangle;
    private float rotation;

    public ButtonView(Texture2D buttonTexture, Vector2 position, Vector2 size, bool visible, string? text = null, SpriteFont? font = null, int iconSpriteId = -1, Vector2 iconPosition = default, string? toolTipText = null)
    {
        this.buttonTexture = buttonTexture;
        Position = position;
        Size = size;
        Visible = visible;
        this.text = text;
        this.font = font;
        this.iconSpriteId = iconSpriteId;
        this.iconPosition = iconPosition;
        this.toolTipText = toolTipText;
        buttonRectangle = CreateButtonRectangle(position, size);
    }

    private static Rectangle CreateButtonRectangle(Vector2 position, Vector2 size) =>
        new((int)(position.X - size.X / CenterFactor), (int)(position.Y - size.Y / CenterFactor), (int)size.X, (int)size.Y);

    public override bool Interact(InputStateModel inputState)
    {
        if (!Visible) return false;

        isHovered = buttonRectangle.Contains(inputState.MousePosition);
        if (isHovered && inputState.MouseAction is IActionType.Basic { BasicAction: BasicActionType.Select })
        {
            OnClick?.Invoke();
            return true;
        }
        return false;
    }

    public void SetText(string text)
    {
        this.text = text;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (!Visible) return;

        spriteBatch.Draw(buttonTexture, Position, null, GetButtonColor(), 0f, new Vector2(buttonTexture.Width / CenterFactor, buttonTexture.Height / CenterFactor), Vector2.One, SpriteEffects.None, 0f);

        if (iconSpriteId != -1)
            spriteBatch.Draw(AssetController.SpriteSheet, iconPosition, AssetController.GetRectangle(iconSpriteId),
                GetButtonColor(), 0f, GameController.Origin, GameController.Scale / IconScaleFactor, SpriteEffects.None, 0f);

        if (text != null)
            spriteBatch.DrawString(font, text, Position, Color.Black, rotation, font!.MeasureString(text) / CenterFactor, Vector2.One, SpriteEffects.None, 0f);

        if (toolTipText != null && isHovered)
            spriteBatch.DrawString(font, toolTipText, new Vector2(GameController.ScreenWidth / 2f - 100, font!.MeasureString(text).Y * 1.5f),
                Color.White, 0f, font.MeasureString(text) / CenterFactor, Vector2.One, SpriteEffects.None, 0f);
    }

    public void RotateText(float rotationAngle)
    {
        rotation = rotationAngle;
    }

    private Color GetButtonColor()
    {
        if (isHovered) return Color.White * HoverOpacity;
        return Color.White;
    }

    public void Subscribe(Action buttonAction)
    {
        OnClick += buttonAction;
    }
}