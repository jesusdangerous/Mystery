using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MysteryWorld.Models.Enums;

namespace MysteryWorld.Controllers;

public sealed class AssetController
{
    internal static Texture2D SpriteSheet;
    private readonly Dictionary<(AssetTypes, Language), Texture2D> assets = new();

    internal readonly Texture2D pausePaneBackground;
    internal readonly Texture2D mainMenuPaneBackground;

    internal readonly Texture2D hudPanelBackground;
    internal static Texture2D hudPanelBar;

    internal static SpriteFont hudFont;
    internal readonly SpriteFont font;

    internal readonly Texture2D gameOverScreenBackground;
    internal readonly Texture2D gameWonScreenBackground;

    internal readonly Texture2D upgradeBackground;

    internal AssetController(ContentManager content)
    {
        var newGameButtonTextureEnglish = content.Load<Texture2D>("newGame");
        var exitButtonTextureEnglish = content.Load<Texture2D>("exit");

        var continueButtonTextureEnglish = content.Load<Texture2D>("continueButtonENG");

        gameOverScreenBackground = content.Load<Texture2D>("gameOver");
        gameWonScreenBackground = content.Load<Texture2D>("gameWin");

        hudFont = content.Load<SpriteFont>("font");
        SpriteSheet = content.Load<Texture2D>("AnimationSpriteSheet");

        pausePaneBackground = content.Load<Texture2D>(assetName: "background2New2");

        mainMenuPaneBackground = content.Load<Texture2D>("menu");

        hudPanelBar = content.Load<Texture2D>(assetName: "lifebar");

        font = content.Load<SpriteFont>("File");

        assets.Add((AssetTypes.NewGameButton, Language.English), newGameButtonTextureEnglish);
        assets.Add((AssetTypes.ExitButton, Language.English), exitButtonTextureEnglish);

        assets.Add((AssetTypes.ContinueButton, Language.English), continueButtonTextureEnglish);
    }

    internal static Rectangle GetRectangle(int spriteId) =>
        new(spriteId % 64 * 16, spriteId / 64 * 16, 16, 16);

    public Texture2D GetTranslatedAsset(AssetTypes type) =>
        assets[(type, GameController.Language)];
}