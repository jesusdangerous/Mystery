using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MysteryWorld.Controllers;
using MysteryWorld.Models.Enums;

namespace MysteryWorld.Views;

public class TargetVisualizerView
{
    public static void Draw(SpriteBatch spriteBatch, LevelController levelState)
    {
        if (levelState.IsInTechnicalState && !levelState.IsInAiState) return;

        if (levelState.Summoner.Path.Count > 0 && levelState.Summoner.CurrentState != CharacterState.Attacking)
            DrawTarget(spriteBatch, levelState.Summoner.Path[0]);

        foreach (var summonedEntity in levelState.FriendlySummons.Values)
            if (summonedEntity.Path.Count > 0 && summonedEntity.CurrentState != CharacterState.Attacking)
                DrawTarget(spriteBatch, summonedEntity.Path[0]);
    }

    private static void DrawTarget(SpriteBatch spriteBatch, Vector2 position)
    {
        spriteBatch.Draw(AssetController.SpriteSheet, position, AssetController.GetRectangle(3048), Color.White, 0f,
            GameController.Origin, GameController.Scale, SpriteEffects.None, 0.45f);
    }
}