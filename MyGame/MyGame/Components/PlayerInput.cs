using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Components
{
    class PlayerInput : Component
    {
        public override ComponentType ComponentType
        {
            get { return ComponentType.PlayerInput;  }
        }

        public PlayerInput() 
        {
            ManagerInput.FireNewInput += ManagerInput_FireNewInput;
        }

        void ManagerInput_FireNewInput(object sender, MyEventArgs.NewInputEventArgs e)
        {
            var sprite = GetComponent<Sprite>(ComponentType.Sprite);

            if (sprite == null)
                return;

            var collision = GetComponent<Collision>(ComponentType.Collision);

            var x = 0f;
            var y = 0f;

            switch (e.Input)
            {
                case Input.Up:
                    y = -1.5f;
                    break;

                case Input.Down:
                    y = 1.5f;
                    break;

                case Input.Left:
                    x = -1.5f;
                    break;

                case Input.Right:
                    x = 1.5f;
                    break;
            }

            if (collision == null || !collision.CheckCollision(new Rectangle((int) (sprite.Position.X + x), (int) (sprite.Position.Y + y), sprite.Width, sprite.Height)))
                sprite.Move(x, y);
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            
        }

        public override void Update(double gameTime)
        {
            
        }
    }
}
