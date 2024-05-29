using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace MysteryWorld.Views;

public static class SpriteBatchView
{
    private static Texture2D spriteBlankTexture;

    private static Texture2D BlankTexture(this GraphicsResource s)
    {
        if (spriteBlankTexture != null) return spriteBlankTexture;

        spriteBlankTexture = new Texture2D(s.GraphicsDevice, 1, 1);
        spriteBlankTexture.SetData(new[] { Color.White });
        return spriteBlankTexture;
    }

    internal static void DrawRectangle(this SpriteBatch sprite, Rectangle rectangle, int linewidth, Color color, float layer = 0f)
    {
        sprite.Draw(sprite.BlankTexture(), new Rectangle(rectangle.X, rectangle.Y, linewidth, rectangle.Height + linewidth), null, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
        sprite.Draw(sprite.BlankTexture(), new Rectangle(rectangle.X, rectangle.Y, rectangle.Width + linewidth, linewidth), null, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
        sprite.Draw(sprite.BlankTexture(), new Rectangle(rectangle.X + rectangle.Width, rectangle.Y, linewidth, rectangle.Height + linewidth), null, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
        sprite.Draw(sprite.BlankTexture(), new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width + linewidth, linewidth), null, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
    }
}