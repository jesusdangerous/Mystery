using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MysteryWorld.Models.Enums;
using MysteryWorld.Controllers;
using MysteryWorld.Models;
using MysteryWorld.Models.Interfaces;

namespace MysteryWorld.Views;

internal sealed class PauseView : MenuView
{
    private readonly AssetController assetManager;

    public PauseView(AssetController assetManager, EventController eventDispatcher)
    {
        DrawLower = true;
        EventDispatcher = eventDispatcher;
        this.assetManager = assetManager;
        CreateMenuElements();
    }

    protected override void CreateMenuElements()
    {
        var panelPosition = GameController.Center;
        var standardButtonSize = new Vector2(128, 64);

        var continueButton = new ButtonView(assetManager.GetTranslatedAsset(AssetTypes.ContinueButton), panelPosition + new Vector2(0, -60), standardButtonSize, true);
        continueButton.Subscribe(OnContinueButtonClick);

        var exitButton = new ButtonView(assetManager.GetTranslatedAsset(AssetTypes.ExitButton), panelPosition + new Vector2(0, 60), standardButtonSize, true);
        exitButton.Subscribe(OnExitButtonClick);

        MenuElements.Add(new MenuPanelView(new List<MenuElementModel>() { continueButton, exitButton }, Vector2.Zero, panelPosition, assetManager.pausePaneBackground));
    }

    private void OnContinueButtonClick()
    {
        EventDispatcher.SendScreenRequest(new INavigationEvent.PopScreen());
    }

    private void OnExitButtonClick()
    {
        EventDispatcher.SendScreenRequest(new INavigationEvent.PopAll());
        EventDispatcher.SendScreenRequest(new INavigationEvent.MainMenu());
    }

    public override void HandleInput(InputStateModel inputState)
    {
        if (inputState.KeyAction is IActionType.Basic { BasicAction: BasicActionType.Escape })
        {
            EventDispatcher.SendScreenRequest(new INavigationEvent.PopScreen());
            return;
        }
        base.HandleInput(inputState);
    }
}