using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyGame.Components;
using MyGame.Manager;

namespace MyGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private BaseObject _player;
        private ManagerInput _managerInput;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            this.graphics.PreferredBackBufferHeight = 240;
            this.graphics.PreferredBackBufferWidth = 320;
            _player = new BaseObject();
            _managerInput = new ManagerInput();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            _player.AddComponent(new Sprite(Content.Load<Texture2D>("player_full"), 16, 21, new Vector2(50, 50)));
            _player.AddComponent(new PlayerInput());
            _player.AddComponent(new Animation(16, 16));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _managerInput.Update(gameTime.ElapsedGameTime.Milliseconds);
            _player.Update(gameTime.ElapsedGameTime.Milliseconds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(196, 207, 161));

            spriteBatch.Begin();
            _player.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
