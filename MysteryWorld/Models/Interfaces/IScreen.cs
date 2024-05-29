using Microsoft.Xna.Framework.Graphics;

namespace MysteryWorld.Models.Interfaces;

internal interface IScreen
{
    bool UpdateLower { get; }
    bool DrawLower { get; }
    void Update(float deltaTime);
    void Draw(SpriteBatch spriteBatch);
    void HandleInput(InputStateModel inputState);
    void RebuildScreen();
}