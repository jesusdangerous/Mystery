using MysteryWorld.Models.Interfaces;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace MysteryWorld.Models;

public sealed class InputStateModel
{
    internal Vector2 MousePosition { get; }
    internal IActionType KeyAction { get; }
    internal IActionType MouseAction { get; }

    internal InputStateModel(Vector2 mousePosition, IActionType mouseAction, IActionType keyAction)
    {
        MousePosition = mousePosition;
        KeyAction = keyAction;
        MouseAction = mouseAction;
    }
}