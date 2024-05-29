using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MysteryWorld.Controllers;
using MysteryWorld.Models;
using MysteryWorld.Models.Enums;

namespace MysteryWorld
{
    internal sealed class GameController : Game
    {
        public static int ScreenWidth = 1024;
        public static int ScreenHeight = 768;
        public static Vector2 Center;
        public static Vector2 Scale = new(4f, 4f);
        private const float PixelSize = 16;
        public static readonly float ScaledPixelSize = (int)(PixelSize * Scale.X);
        public static Vector2 Origin = new(PixelSize / 2, PixelSize / 2);
        public static bool DebugMode;
        public static bool FullScreen;
        public static Language Language = Language.English;

        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;

        private ScreenController screenManager;
        private InputMapperController inputMapper;
        private EventController eventDispatcher;

        private AssetController assetManager;
        private TemporaryController popupManager;
        private FrameCounterModel frameCounter;

        public GameController()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Center = new Vector2(ScreenWidth / 2f, ScreenHeight / 2f);
            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.PreferredBackBufferHeight = ScreenHeight;
            graphics.ApplyChanges();

            eventDispatcher = new EventController();
            inputMapper = new InputMapperController();
            assetManager = new AssetController(Content);
            popupManager = new TemporaryController(eventDispatcher, assetManager);
            screenManager = new ScreenController(new AbstractFactory(assetManager, eventDispatcher), eventDispatcher);
            frameCounter = new FrameCounterModel();

            eventDispatcher.OnExit += Exit;
            eventDispatcher.OnResolutionRequest += HandleResize;
            eventDispatcher.OnFullScreenRequest += HandleFullScreen;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = Content.Load<SpriteFont>("FramecounterFont");
        }

        protected override void Update(GameTime gameTime)
        {
            if (!IsActive) return;

            var input = inputMapper.Update();
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            popupManager.Update(deltaTime);
            screenManager.Update(deltaTime, input);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            screenManager.Draw(spriteBatch);
            popupManager.Draw(spriteBatch);
            if (DebugMode)
            {
                frameCounter.Draw(spriteBatch, spriteFont);
                frameCounter.Update();
            }

            base.Draw(gameTime);
        }

        private void HandleResize(ResolutionEventModel resEvent)
        {
            graphics.IsFullScreen = false;
            FullScreen = false;
            graphics.PreferredBackBufferWidth = resEvent.Width;
            graphics.PreferredBackBufferHeight = resEvent.Height;
            ScreenWidth = resEvent.Width;
            ScreenHeight = resEvent.Height;
            Center = new Vector2(graphics.PreferredBackBufferWidth / 2f, graphics.PreferredBackBufferHeight / 2f);
            graphics.ApplyChanges();
            screenManager.RebuildScreens();
        }

        private void HandleFullScreen()
        {
            ScreenWidth = Math.Min(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 1920);
            ScreenHeight = Math.Min(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height, 1080);
            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.PreferredBackBufferHeight = ScreenHeight;
            Center = new Vector2(graphics.PreferredBackBufferWidth / 2f, graphics.PreferredBackBufferHeight / 2f);
            graphics.IsFullScreen = true;
            FullScreen = true;
            graphics.HardwareModeSwitch = false;
            graphics.ApplyChanges();
            screenManager.RebuildScreens();
        }
    }
}