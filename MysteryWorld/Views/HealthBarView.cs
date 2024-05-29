using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MysteryWorld.Models;

namespace MysteryWorld.Views;

public class HealthBarView : MenuElementModel
{
    private readonly Texture2D barTexture;
    private readonly Rectangle barRectangle;

    public override bool Interact(InputStateModel inputState) => false;

    public HealthBarView(Texture2D barTexture, Rectangle barRectangle)
    {
        this.barTexture = barTexture;
        this.barRectangle = barRectangle;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(barTexture, barRectangle, Color.White);
    }
}