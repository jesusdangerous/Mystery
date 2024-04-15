﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Components
{
    class Sprite : Component
    {
        private Texture2D _texture;
        public int Width {  get; private set; }
        public int Height {  get; private set; }
        public Vector2 Position {  get; private set; }

        public Sprite(Texture2D texture, int width, int height, Vector2 position)
        {
            _texture = texture;
            Width = width;
            Height = height;
            Position = position;
        } 

        public override ComponentType ComponentType
        {
            get { return ComponentType.Sprite; }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var animation = GetComponent<Animation>(ComponentType.Animation);
            if (animation != null)
            {
                spriteBatch.Draw(_texture, new Rectangle((int)Position.X, (int)Position.Y, Width, Height), animation.TextureRectangle, Color.White);
            }
            else
                spriteBatch.Draw(_texture, new Rectangle((int)Position.X, (int)Position.Y, Width, Height), Color.White);
        }

        public override void Update(double gameTime)
        {

        }

        public void Move(float x, float y)
        {
            Position = new Vector2(Position.X + x, Position.Y + y);
            var animation = GetComponent<Animation>(ComponentType.Animation);
            if (animation == null)
                return;

            if (x > 0)
            {
                animation.ResetComputer(State.Walking, Direction.Right);
            }
            else if (x < 0)
            {
                animation.ResetComputer(State.Walking, Direction.Left);
            }
            else if (y > 0)
            {
                animation.ResetComputer(State.Walking, Direction.Down);
            }
            else if (y < 0)
            {
                animation.ResetComputer(State.Walking, Direction.Up);
            }
        }
    }
}
