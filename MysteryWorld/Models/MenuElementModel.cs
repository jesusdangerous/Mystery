using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MysteryWorld.Models;

public abstract class MenuElementModel
{
    public Vector2 Position { get; protected init; }
    protected Vector2 Size { get; init; }
    protected bool Visible { get; set; }
    public abstract bool Interact(InputStateModel inputState);
    public abstract void Draw(SpriteBatch spriteBatch);
}