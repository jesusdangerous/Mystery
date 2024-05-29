using Microsoft.Xna.Framework.Input;

namespace MysteryWorld.Controllers;

public class KeyboardController
{
    private KeyboardState oldKeyState;
    private KeyboardState currentKeyState;

    public KeyboardController()
    {
        oldKeyState = Keyboard.GetState();
        currentKeyState = Keyboard.GetState();
    }

    public void Update()
    {
        oldKeyState = currentKeyState;
        currentKeyState = Keyboard.GetState();
    }

    public bool WasPressed(Keys key) =>
       !oldKeyState.IsKeyDown(key) && currentKeyState.IsKeyDown(key);
}