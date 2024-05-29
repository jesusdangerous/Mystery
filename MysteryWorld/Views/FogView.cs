using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using MysteryWorld.Models;
using MysteryWorld.Controllers;

namespace MysteryWorld.Views
{
    public sealed class FogView
    {
        private const float F04 = 0.4f;

        internal bool[,] FogMask { get; set; }
        private int Size { get; set; }
        internal bool Use { get; set; }

        private FogView()
        {
        }

        public static FogView CreateFogOfWar(MapModel map, bool use)
        {
            var fog = new FogView
            {
                Size = map.DungeonDimension
            };
            fog.FogMask = new bool[fog.Size, fog.Size];
            fog.ResetTo(true);
            fog.Use = use;
            return fog;
        }

        private void ResetTo(bool desired)
        {
            for (var y = 0; y < Size; y++)
                for (var x = 0; x < Size; x++)
                    FogMask[y, x] = desired;
        }

        internal void Update(Vector2 position, int updateRange)
        {
            if (!Use) return;
            var gridPos = position.ToGrid();

            for (var y = Math.Clamp(gridPos.Y - updateRange + 1, 0, Size); y < Math.Clamp(gridPos.Y + updateRange, 0, Size); y++)
                for (var x = Math.Clamp(gridPos.X - updateRange + 1, 0, Size); x < Math.Clamp(gridPos.X + updateRange, 0, Size); x++)
                    FogMask[(int)x, (int)y] = false;
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle visibleMapArea)
        {
            if (GameController.DebugMode || !Use)
                return;

            for (var y = Math.Max(0, visibleMapArea.Top); y < Math.Min(Size - 1, visibleMapArea.Bottom); y++)
                for (var x = Math.Max(0, visibleMapArea.Left); x < Math.Min(Size - 1, visibleMapArea.Right); x++)
                    if (FogMask[x, y])
                    {
                        spriteBatch.Draw(AssetController.SpriteSheet, new Vector2(GameController.ScaledPixelSize * x, GameController.ScaledPixelSize * y),
                            AssetController.GetRectangle(16), Color.White, 0f, Vector2.Zero, GameController.Scale, SpriteEffects.None, F04);
                    }
        }
    }
}