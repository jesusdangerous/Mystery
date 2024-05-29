using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MysteryWorld.Controllers;
using MysteryWorld.Models;
using MysteryWorld.Models.Enums;
using MysteryWorld.Models.Interfaces;

namespace MysteryWorld.Views;

internal sealed class MainMenuView : MenuView
{
    private readonly AssetController assetManager;

    public MainMenuView(AssetController assetManager, EventController eventDispatcher)
    {
        EventDispatcher = eventDispatcher;
        this.assetManager = assetManager;
        CreateMenuElements();
    }

    protected override void CreateMenuElements()
    {
        BackgroundTexture = assetManager.mainMenuPaneBackground;
        var panelPosition = GameController.Center - new Vector2(0, 115);
        var offset = new Vector2(0, 66);

        var newGameButton = new ButtonView(assetManager.GetTranslatedAsset(AssetTypes.NewGameButton), panelPosition, new Vector2(128, 64), true);
        newGameButton.Subscribe(OnNewGameButtonClick);

        var exitButton = new ButtonView(assetManager.GetTranslatedAsset(AssetTypes.ExitButton), panelPosition + 2 * offset, new Vector2(128, 64), true);

        exitButton.Subscribe(OnExitButtonClick);

        MenuElements.Add(new MenuPanelView(new List<MenuElementModel>() { newGameButton, exitButton }, Vector2.Zero, panelPosition));
    }

    private void OnNewGameButtonClick()
    {
        EventDispatcher.SendScreenRequest(new INavigationEvent.PopScreen());
        EventDispatcher.SendScreenRequest(new INavigationEvent.NewGame(LevelController.CreateDefaultLevelState()));
    }

    private void OnExitButtonClick()
    {
        EventDispatcher.CloseGame();
    }
}