using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using MysteryWorld.Models;
using MysteryWorld.Models.Enums;
using MysteryWorld.Models.Interfaces;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace MysteryWorld.Controllers;

internal sealed class GameInputController
{
    private const int FloorLimit = 5;
    private const float ZoomDelta = 0.1f;

    private readonly EventController eventDispatcher;
    private readonly LevelController levelState;
    private readonly GameModel gameLogic;
    private readonly HighlightRendererModel highlightRenderer;

    public GameInputController(LevelController levelState, EventController eventDispatcher, GameModel gameLogic, HighlightRendererModel selectionRenderer)
    {
        this.levelState = levelState;
        this.gameLogic = gameLogic;
        this.eventDispatcher = eventDispatcher;
        highlightRenderer = selectionRenderer;
    }

    public void HandleInput(InputStateModel inputState)
    {
        switch (inputState.MouseAction)
        {
            case IActionType.Basic basic:
                HandleBasicAction(basic, inputState);
                break;
        }

        switch (inputState.KeyAction)
        {
            case IActionType.Basic basic:
                HandleBasicAction(basic, inputState);
                break;
            case IActionType.Summon summon:
                HandleToggleSummonModeAction(summon.SummonType);
                break;
            case IActionType.Select select:
                HandleToggleUnitAction(select);
                break;
        }
    }

    private void HandleBasicAction(IActionType.Basic basic, InputStateModel inputState)
    {
        switch (basic.BasicAction)
        {
            case BasicActionType.Command:
                var selection = levelState.FriendlySummons.Values.Where(c => c.Selected).Cast<CharacterController>().ToList();
                if (levelState.Summoner.Selected)
                    selection.Add(levelState.Summoner);

                HandleRightClickAction(selection, inputState.MousePosition);
                break;
            case BasicActionType.Select:
                HandleLeftClickAction(inputState);
                break;
            case BasicActionType.DragSelect:
                HandleSelectionRectangle();
                break;
            case BasicActionType.DamageSpell:
                HandleDamageSpellAction(inputState.MousePosition);
                break;
            case BasicActionType.Interact:
                HandleInteractAction();
                break;
            case BasicActionType.JumpToPlayer:
                HandleJumpToPlayerAction();
                break;
            case BasicActionType.Escape:
                HandleEscapeAction();
                break;
            case BasicActionType.ZoomIn:
                levelState.Camera2d.AdjustZoom(ZoomDelta);
                break;
            case BasicActionType.ZoomOut:
                levelState.Camera2d.AdjustZoom(-ZoomDelta);
                break;
            case BasicActionType.NextLevel:
                if (GameController.DebugMode && levelState.LevelCount < FloorLimit && !levelState.IsInTechnicalState)
                {
                    levelState.LevelCount++;
                    levelState.ChangeLevel();
                }
                break;
        }
    }

    private void HandleRightClickAction(List<CharacterController> characters, Vector2 mousePosition)
    {
        if (characters.Count == 0) return;

        var target = gameLogic.FindTarget(levelState.Camera2d.CameraToWorld(mousePosition));
        if (target is { IsFriendly: false })
        {
            foreach (var character in characters)
            {
                character.CurrentState = CharacterState.Attacking;
                gameLogic.AttackCharacter(character, target);
                character.Selected = false;
            }
            return;
        }

        foreach (var character in characters)
            character.CurrentState = CharacterState.PlayerControl;

        gameLogic.MoveCharacters(characters, levelState.Camera2d.CameraToWorld(mousePosition));
    }
    private void HandleLeftClickAction(InputStateModel inputState)
    {
        if (levelState.Summoner.SelectedSummonType == null)
        {
            if (levelState.QuadTree.PointSearchCharacters(levelState.Camera2d.CameraToWorld(inputState.MousePosition))
                    .Where(character => character.IsFriendly).ToList().Count == 0)
            {
                DeselectFriendlies();
                return;
            }

            levelState.ActionForFriendlyCharacters((c) => HandleSelectAction(c, inputState.MousePosition));
            return;
        }

        if (levelState.IsSummonLimitReached()) return;

        gameLogic.SummonFriendlyMonster(levelState.Summoner.SelectedSummonType, levelState.Camera2d.CameraToWorld(inputState.MousePosition));
    }

    private void HandlePauseGame()
    {
        eventDispatcher.SendScreenRequest(new INavigationEvent.PauseMenu(!levelState.IsInTechnicalState));
    }

    private void HandleDamageSpellAction(Vector2 mousePosition)
    {
        if (levelState.Summoner.Selected)
            gameLogic.ShootFireball(levelState.Camera2d.CameraToWorld(mousePosition));
    }

    private void HandleInteractAction()
    {
        gameLogic.ConsumeItem(levelState.Summoner);
        if (levelState.Ladder?.IntersectsWith(levelState.Summoner) == true && !levelState.IsInTechnicalState)
        {
            levelState.ChangeLevel();
            return;
        }

        if (!levelState.Summoner.IntersectsWith(levelState.BloodShrine)) return;
        levelState.Summoner.StopMovement();

        levelState.Camera2d.IsLocked = true;
    }

    private void HandleJumpToPlayerAction()
    {
        levelState.Camera2d.SetCameraPosition(levelState.Summoner.Position);
    }

    private void HandleToggleSummonModeAction(SummonType? summonType)
    {

        if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
        {
            SelectBySummonType(summonType);
            return;
        }

        if (levelState.Summoner.SelectedSummonType == summonType || !levelState.Summoner.Selected)
        {
            levelState.Summoner.SelectedSummonType = null;
            return;
        }
        levelState.Summoner.SelectedSummonType = summonType;
    }

    private void HandleSelectAction(CharacterController character, Vector2 mousePosition)
    {
        if (character.Hitbox.Contains(levelState.Camera2d.CameraToWorld(mousePosition)))
            character.Selected = !character.Selected;
    }

    private void HandleToggleUnitAction(IActionType.Select selectAction)
    {
        if (levelState.Summoner.SelectedSummonType != null) return;

        var index = selectAction.SelectedIndex;
        if (index == 0)
        {
            gameLogic.SelectCharacter(levelState.Summoner);
            return;
        }

        if (levelState.FriendlySummons.Count <= index - 1) return;

        gameLogic.SelectCharacter(levelState.FriendlySummons.Values.ToList()[index - 1]);
    }

    private void HandleEscapeAction()
    {
        if (levelState.Summoner.SelectedSummonType != null)
            HandleToggleSummonModeAction(levelState.Summoner.SelectedSummonType);
        else HandlePauseGame();
    }

    private void HandleSelectionRectangle()
    {
        var gameObjects = levelState.QuadTree.Search(highlightRenderer.CalculateSelectionBounds());

        DeselectFriendlies();
        foreach (var obj in gameObjects)
            if (obj is CharacterController { IsFriendly: true } character)
                character.Selected = true;
    }

    private void DeselectFriendlies()
    {
        levelState.Summoner.Selected = false;
        levelState.Summoner.SelectedSummonType = null;
        foreach (var friendly in levelState.FriendlySummons)
            friendly.Value.Selected = false;
    }

    private static void SelectBySummonType(SummonType? summonType)
    {
        if (summonType == null) return;

        switch (summonType)
        {
            case SummonType.MagicSeedling:
                break;
            case null:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(summonType), summonType, null);
        }
    }
}