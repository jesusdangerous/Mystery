#nullable enable
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MysteryWorld.Models;

namespace MysteryWorld.Views;

public sealed class MenuPanelView : MenuElementModel
{
    private const float CenterFactor = 2f;

    private readonly List<MenuElementModel> menuElements;
    private readonly Texture2D? panelTexture;

    public MenuPanelView(List<MenuElementModel> menuElements, Vector2 size, Vector2 position, Texture2D? paneTexture = null)
    {
        this.menuElements = menuElements ?? throw new ArgumentNullException(nameof(menuElements));
        panelTexture = paneTexture;
        Size = size;
        Position = position;
    }

    public override bool Interact(InputStateModel inputState)
    {
        foreach (var element in menuElements)
            if (element.Interact(inputState)) return true;
        return false;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (panelTexture != null)
            DrawPanelBackground(spriteBatch, Position, Size, panelTexture);

        foreach (var menuElement in menuElements)
            menuElement.Draw(spriteBatch);
    }

    public void Draw(SpriteBatch spriteBatch, Rectangle rectangle)
    {
        if (panelTexture != null)
            spriteBatch.Draw(panelTexture, rectangle, Color.White);

        foreach (var menuElement in menuElements)
            menuElement.Draw(spriteBatch);
    }

    private static void DrawPanelBackground(SpriteBatch spriteBatch, Vector2 position, Vector2 size, Texture2D texture)
    {
        var origin = new Vector2(texture.Width / CenterFactor, texture.Height / CenterFactor);
        spriteBatch.Draw(texture, position, null, Color.White, 0f, origin, Vector2.One, SpriteEffects.None, 0f);
    }
}