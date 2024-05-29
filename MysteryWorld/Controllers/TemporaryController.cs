using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MysteryWorld.Models;
using MysteryWorld.Models.Interfaces;

namespace MysteryWorld.Controllers;

public sealed class TemporaryController
{
    private const float CenterFactor = 2f;

    private readonly Queue<TemporaryModel> popups = new();
    private readonly AssetController assetManager;

    public TemporaryController(EventController eventDispatcher, AssetController assetManager)
    {
        eventDispatcher.OnPopupEvent += HandlePopupEvent;
        this.assetManager = assetManager;
    }

    private void HandlePopupEvent(IPopupEvent popupEvent)
    {
        if (popupEvent is IPopupEvent.NotificationPopup notificationPopup)
            HandleNotificationPopup(notificationPopup.Notification, notificationPopup.Color, notificationPopup.DisplayDuration);
    }

    private void HandleNotificationPopup(string notification, Color notificationColor, float displayDuration)
    {
        var popup = new TemporaryModel(notification, notificationColor, GameController.Center, displayDuration);
        popups.Enqueue(popup);
    }

    public void Update(float deltaTime)
    {
        if (popups.Count == 0) return;

        var popup = popups.Peek();
        popup.Timer += deltaTime;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (popups.Count == 0) return;

        spriteBatch.Begin();
        spriteBatch.DrawString(assetManager.font, popups.Peek().Message, popups.Peek().Position, popups.Peek().TextColor, 0f,
            assetManager.font.MeasureString(popups.Peek().Message) / CenterFactor, Vector2.One, SpriteEffects.None, 0f);

        spriteBatch.End();
    }
}