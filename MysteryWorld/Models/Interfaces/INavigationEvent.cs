using MysteryWorld.Controllers;

namespace MysteryWorld.Models.Interfaces;

public interface INavigationEvent
{
    public class NewGame : INavigationEvent
    {
        public readonly LevelController mLevelState;
        public NewGame(LevelController levelState)
        {
            mLevelState = levelState;
        }
    }

    public class MainMenu : INavigationEvent
    {
    }

    public class PauseMenu : INavigationEvent
    {
        internal bool isCanSave { get; }

        internal PauseMenu(bool canSave)
        {
            isCanSave = canSave;
        }
    }

    public class PopScreen : INavigationEvent
    {
    }

    public class GameOverScreen : INavigationEvent
    {
    }

    public class ControlsImageScreen : INavigationEvent
    {
        public int BackgroundIndex { get; }

        public ControlsImageScreen(int backgroundIndex)
        {
            BackgroundIndex = backgroundIndex;
        }
    }

    public class GameWonScreen : INavigationEvent
    {

    }

    public class PopAll : INavigationEvent
    {
    }
}
