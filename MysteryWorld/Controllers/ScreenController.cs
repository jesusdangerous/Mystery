using Microsoft.Xna.Framework.Graphics;
using MysteryWorld.Models;
using MysteryWorld.Models.Interfaces;
using System;
using System.Collections.Generic;

namespace MysteryWorld.Controllers;

internal sealed class ScreenController
{
    private readonly List<IScreen> screenStack;
    private readonly AbstractFactory screenFactory;

    internal ScreenController(AbstractFactory screenFactory, EventController eventDispatcher)
    {
        eventDispatcher.OnScreenRequest += HandleScreenRequest;

        screenStack = new List<IScreen>();
        this.screenFactory = screenFactory;
        PushScreen(this.screenFactory.CreateMainMenuScreen());
    }

    private void PushScreen(IScreen screen)
    {
        screenStack.Add(screen);
    }

    private void PopScreen()
    {
        if (screenStack.Count > 0)
            screenStack.RemoveAt(screenStack.Count - 1);
    }

    internal void Update(float deltaTime, InputStateModel action)
    {
        var screenDepth = screenStack.Count;

        if (screenDepth > 0)
        {
            for (var i = 1; i < screenDepth; i++)
                if (screenStack[i].UpdateLower)
                    screenStack[i - 1].Update(deltaTime);

            screenStack[^1].Update(deltaTime);
            screenStack[^1].HandleInput(action);
        }
    }

    internal void Draw(SpriteBatch spriteBatch)
    {
        var screenDepth = screenStack.Count;
        if (screenDepth > 0)
        {
            for (var i = 1; i < screenDepth; i++)
                if (screenStack[i].DrawLower)
                    screenStack[i - 1].Draw(spriteBatch);

            screenStack[screenDepth - 1].Draw(spriteBatch);
        }
    }

    internal void RebuildScreens()
    {
        var screenDepth = screenStack.Count;
        if (screenDepth > 0)
            for (var i = screenDepth - 1; i >= 0; i--)
                screenStack[i].RebuildScreen();
    }

    private void ClearAllScreens()
    {
        for (var i = screenStack.Count - 1; i >= 0; i--)
            screenStack[i].RebuildScreen();
    }

    private void HandleScreenRequest(INavigationEvent navigationEvent)
    {
        switch (navigationEvent)
        {
            case INavigationEvent.NewGame e:
                PushScreen(screenFactory.CreateGameScreen(e.mLevelState));
                break;
            case INavigationEvent.MainMenu:
                PushScreen(screenFactory.CreateMainMenuScreen());
                break;
            case INavigationEvent.PauseMenu e:
                PushScreen(screenFactory.CreatePauseMenu());
                break;
            case INavigationEvent.PopScreen:
                PopScreen();
                break;
            case INavigationEvent.PopAll:
                ClearAllScreens();
                break;
            case INavigationEvent.GameOverScreen:
                PushScreen(screenFactory.CreateGameOverScreen());
                break;
            case INavigationEvent.GameWonScreen:
                PushScreen(screenFactory.CreateGameWonScreen());
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(navigationEvent), navigationEvent, null);
        }
    }
}