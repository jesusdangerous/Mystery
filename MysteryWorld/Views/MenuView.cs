using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MysteryWorld.Controllers;
using MysteryWorld.Models;
using MysteryWorld.Models.Interfaces;

namespace MysteryWorld.Views;

public abstract class MenuView : IScreen
{
    protected readonly List<MenuElementModel> MenuElements = new();
    protected EventController EventDispatcher;
    protected Texture2D BackgroundTexture = null;
    protected bool ScaleBackground = true;
    public bool UpdateLower { get; set; }
    public bool DrawLower { get; set; }

    public virtual void Update(float deltaTime)
    {
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
        if (BackgroundTexture != null && ScaleBackground)
            spriteBatch.Draw(BackgroundTexture, new Rectangle(0, 0, GameController.ScreenWidth, GameController.ScreenHeight), Color.White);
        else if (BackgroundTexture != null)
            spriteBatch.Draw(BackgroundTexture, GameController.Center, null, Color.White, 0f, new Vector2(BackgroundTexture.Width / 2f, BackgroundTexture.Height / 2f), Vector2.One, SpriteEffects.None, 0f);

        foreach (var menuElement in MenuElements)
            menuElement.Draw(spriteBatch);

        spriteBatch.End();
    }

    public virtual void HandleInput(InputStateModel inputState)
    {
        foreach (var menuElement in MenuElements)
            if (menuElement.Interact(inputState)) return;
    }

    protected abstract void CreateMenuElements();

    public void RebuildScreen()
    {
        MenuElements.Clear();
        CreateMenuElements();
    }
}