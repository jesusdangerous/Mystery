using System;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using static MysteryWorld.Models.Enums.BasicActionType;
using MysteryWorld.Models;
using MysteryWorld.Models.Enums;
using MysteryWorld.Models.Interfaces;

namespace MysteryWorld.Controllers;
internal sealed class InputMapperController
{
    private const float RectangleThreshold = 8f;

    private readonly KeyboardController keyboardListener;
    private readonly MouseController mouseListener;

    private Vector2 rectangleStart;
    private Vector2 rectangleEnd;
    private readonly Dictionary<KeyEventModel, IActionType> keyActionMapping = new();

    public InputMapperController()
    {
        keyboardListener = new KeyboardController();
        mouseListener = new MouseController();
        keyActionMapping.Add(new KeyEventModel(Keys.A), new IActionType.Basic(DamageSpell));
        keyActionMapping.Add(new KeyEventModel(Keys.F), new IActionType.Basic(Interact));
        keyActionMapping.Add(new KeyEventModel(Keys.Space), new IActionType.Basic(JumpToPlayer));
        keyActionMapping.Add(new KeyEventModel(Keys.Escape), new IActionType.Basic(Escape));
    }

    public InputStateModel Update()
    {
        keyboardListener.Update();
        mouseListener.Update();
        return new InputStateModel(mouseListener.GetMousePosition(), GetMouseAction(), GetKeyboardAction());
    }

    private IActionType GetKeyboardAction()
    {

        foreach (var keyValuePair in keyActionMapping)
        {
            switch (keyValuePair.Key.type)
            {
                case KeyEventTypeEnum.OnButtonDown:
                    break;
                case KeyEventTypeEnum.OnButtonPressed:
                    if (keyboardListener.WasPressed(keyValuePair.Key.key))
                        return keyValuePair.Value;
                    break;
                case KeyEventTypeEnum.OnButtonUp:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        return new IActionType.Basic(None);
    }

    private IActionType GetMouseAction()
    {
        if (mouseListener.WasPressedLmb())
        {
            rectangleStart = mouseListener.GetMousePosition();
            rectangleEnd = mouseListener.GetMousePosition();
        }
        else if (mouseListener.IsHeldLmb())
        {
            rectangleEnd = mouseListener.GetMousePosition();
            return new IActionType.Basic(None);
        }
        else if (mouseListener.WasReleasedLmb())
        {
            if (Vector2.Distance(rectangleStart, rectangleEnd) > RectangleThreshold)
                return new IActionType.Basic(DragSelect);
            return new IActionType.Basic(Select);
        }

        if (mouseListener.WasClickedRmb())
            return new IActionType.Basic(Command);
        if (mouseListener.WasScrolledUp())
            return new IActionType.Basic(ZoomIn);
        if (mouseListener.WasScrolledDown())
            return new IActionType.Basic(ZoomOut);
        return new IActionType.Basic(None);
    }
}
