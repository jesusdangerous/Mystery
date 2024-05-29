using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MysteryWorld.Controllers;

public class MouseController
{
    private MouseState oldMouseState;
    private MouseState currentMouseState;

    public MouseController()
    {
        oldMouseState = Mouse.GetState();
        currentMouseState = Mouse.GetState();
    }

    public void Update()
    {
        oldMouseState = currentMouseState;
        currentMouseState = Mouse.GetState();
    }

    public Vector2 GetMousePosition() =>
        currentMouseState.Position.ToVector2();

    public bool WasPressedLmb() =>
        currentMouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released;

    public bool WasReleasedLmb() => oldMouseState.LeftButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Released;

    public bool IsHeldLmb() =>
        oldMouseState.LeftButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Pressed;

    public bool WasClickedRmb() =>
        currentMouseState.RightButton == ButtonState.Released && oldMouseState.RightButton == ButtonState.Pressed;

    public bool WasScrolledUp() =>
        oldMouseState.ScrollWheelValue < currentMouseState.ScrollWheelValue;

    public bool WasScrolledDown() =>
        oldMouseState.ScrollWheelValue > currentMouseState.ScrollWheelValue;
}