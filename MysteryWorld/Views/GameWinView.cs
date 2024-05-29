using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MysteryWorld.Controllers;
using MysteryWorld.Models;
using MysteryWorld.Models.Enums;
using MysteryWorld.Models.Interfaces;
using static MysteryWorld.GameController;

namespace MysteryWorld.Views;

public sealed class GameWinView : MenuView
{
    private readonly AssetController assetManager;

    public GameWinView(EventController eventDispatcher, AssetController assetManager)
    {
        this.assetManager = assetManager;
        UpdateLower = false;
        DrawLower = true;
        EventDispatcher = eventDispatcher;
        BackgroundTexture = assetManager.gameWonScreenBackground;
        CreateMenuElements();
    }

    protected override void CreateMenuElements()
    {
        var panelPosition = Center;

        var backMenu = new ButtonView(assetManager.GetTranslatedAsset(AssetTypes.MainMenuButton), new Vector2(Center.X, Center.Y + 150), new Vector2(100, 100), true);

        backMenu.Subscribe(OnBackToMenuButtonClick);

        MenuElements.Add(new MenuPanelView(new List<MenuElementModel>() { backMenu }, Vector2.Zero, panelPosition));
    }

    private void OnBackToMenuButtonClick()
    {
        EventDispatcher.SendScreenRequest(new INavigationEvent.PopScreen());
        EventDispatcher.SendScreenRequest(new INavigationEvent.MainMenu());
    }
}