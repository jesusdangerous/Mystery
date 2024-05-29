using Microsoft.Xna.Framework.Graphics;
using MysteryWorld.Models;
using MysteryWorld.Controllers;
using MysteryWorld.Models.Interfaces;

namespace MysteryWorld.Views;

internal sealed class GameView : IScreen
{
    public LevelController CurrentLevelState { get; }
    private readonly GameInputController inputHandler;
    private readonly GameModel gameLogic;
    private readonly HighlightRendererModel highlightRenderer;
    private readonly TargetVisualizerView targetVisualizer;

    public bool UpdateLower => false;
    public bool DrawLower => false;

    internal GameView(LevelController levelState, EventController eventDispatcher, AssetController assetManager)
    {
        CurrentLevelState = levelState;
        CurrentLevelState.EventDispatcher = eventDispatcher;

        gameLogic = new GameModel(levelState, eventDispatcher);
        highlightRenderer = new HighlightRendererModel(levelState.Camera2d);
        inputHandler = new GameInputController(CurrentLevelState, eventDispatcher, gameLogic, highlightRenderer);
        targetVisualizer = new TargetVisualizerView();
    }

    public void Update(float deltaTime)
    {
        CurrentLevelState.UpdateGameObjects(deltaTime);
        CurrentLevelState.UpdateQuadTree();
        CurrentLevelState.UpdateFog();
        gameLogic.Update(deltaTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        DrawGameObjects(spriteBatch);
        DrawHud(spriteBatch);
    }

    private void DrawGameObjects(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(SpriteSortMode.BackToFront, null, SamplerState.PointClamp, transformMatrix: CurrentLevelState.Camera2d.Transform);
        CurrentLevelState.Draw(spriteBatch);
        highlightRenderer.Draw(spriteBatch);
        TargetVisualizerView.Draw(spriteBatch, CurrentLevelState);
        spriteBatch.End();
    }

    private void DrawHud(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
        spriteBatch.End();
    }

    public void HandleInput(InputStateModel inputState)
    {
        highlightRenderer.HandleInput();
        inputHandler.HandleInput(inputState);
    }

    public void RebuildScreen()
    {
        CurrentLevelState.Camera2d.UpdateCamera();
    }
}