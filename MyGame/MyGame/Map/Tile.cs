using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Map
{
    public class Tile
    {
        private const int Width = 16;
        private const int Height = 16;

        public int XPos { get; set; }
        public int YPos { get; set; }
        public int ZPos { get; set; }

        public int TextureXPos { get; set; }
        public int TextureYPos { get; set; }

        public string TextureName { get; set; }
        private Texture2D _texture;

        public Tile()
        {

        }

        public Tile(int xPos, int yPos, int zPos, int textureXPos, int  textureYPos, string textureName)
        {
            XPos = xPos;
            YPos = yPos;
            ZPos = zPos;
            TextureXPos = textureXPos;
            TextureYPos = textureYPos;
            TextureName = textureName;
        }

        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>(TextureName);
        }

        public void Update(double gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch
                .Draw(_texture, 
                new Rectangle(XPos * Width, YPos * Height, Width, Height), 
                new Rectangle(TextureXPos * Width, TextureYPos * Height, Width, Height), Color.White);
        }
    }
}
