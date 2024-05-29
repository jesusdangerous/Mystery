using MysteryWorld.Controllers;
using MysteryWorld.Models.Interfaces;
using MysteryWorld.Views;

namespace MysteryWorld.Models
{
    internal sealed class AbstractFactory
    {
        private readonly AssetController assetManager;
        private readonly EventController eventDispatcher;

        public AbstractFactory(AssetController assetManager, EventController eventDispatcher)
        {
            this.assetManager = assetManager;
            this.eventDispatcher = eventDispatcher;
        }

        internal IScreen CreateMainMenuScreen() =>
            new MainMenuView(assetManager, eventDispatcher);

        internal IScreen CreatePauseMenu() =>
            new PauseView(assetManager, eventDispatcher);

        internal IScreen CreateGameScreen(LevelController levelState) =>
            new GameView(levelState, eventDispatcher, assetManager);

        internal IScreen CreateGameOverScreen() =>
            new GameOverView(eventDispatcher, assetManager);

        internal IScreen CreateGameWonScreen() =>
            new GameWinView(eventDispatcher, assetManager);
    }
}