using System.Security.Cryptography;
using MysteryWorld.Models;
using MysteryWorld.Models.Enums;

namespace MysteryWorld.Controllers
{
    public sealed class GridController
    {
        private const int BloodShrineTopSpriteId = 2;
        private const int BloodShrineBottomSpriteId = 3;

        private const int BloodShrineTopId = 5;
        private const int PillarTopSpriteId = 6;
        private const int PillarMiddleSpriteId = 7;
        private const int PillarBottomSpriteId = 8;
        private const int HorizontalCrackSpriteId = 9;
        private const int SkullAndBoneSpriteId = 10;
        private const int VerticalCrackSpriteId = 11;
        private const int OldBloodStainSpriteId = 12;
        private const int OldBloodStainOnWallSpriteId = 13;
        private const int SkullOnWallSpriteId = 14;
        private const int DestructAbleWallPhase1SpriteId = 18;
        private const int EmptySprite = 31;

        private const int GroundSkullAndBoneChance = 20;
        private const int GroundHorizontalCrackChance = 30;
        private const int GroundOldBloodStainChance = 40;

        private const int WallPillarMiddleChance = 35;
        private const int WallBloodStainOnWallChance = 60;
        private const int WallSkullOnWallChance = 70;

        public CellModel[,] CellGrid;
        public int Width;
        public int Height;

        public GridController(int width, int height)
        {
            CellGrid = new CellModel[height, width];
            for (var i = 0; i < height; i++)
                for (var j = 0; j < width; j++)
                    CellGrid[i, j] = new CellModel(CellTypeEnum.EmptyCell);

            Width = width;
            Height = height;
        }

        internal void SetCell(int x, int y, CellTypeEnum c)
        {
            CellGrid[y, x].CellType = c;
        }

        internal CellTypeEnum GetCellType(int x, int y) =>
            CellGrid[y, x].CellType;

        internal bool[,] CollisionMatrix()
        {
            var result = new bool[Height, Width];
            for (var i = 0; i < Height; i++)
                for (var j = 0; j < Width; j++)
                {
                    if (CellGrid[i, j].CellType == CellTypeEnum.GroundCell || CellGrid[i, j].CellType == CellTypeEnum.ShrineBotCell)
                        result[i, j] = false;
                    else result[i, j] = true;
                }

            return result;
        }

        internal int[,] BackgroundSpriteMatrix()
        {
            var outMatrix = new int[Height, Width];
            for (var i = 0; i < Height; i++)
                for (var j = 0; j < Width; j++)
                    outMatrix[i, j] = BackgroundSprite(CellGrid[i, j].CellType);

            return outMatrix;
        }

        private static int BackgroundSprite(CellTypeEnum cellType)
        {
            var value = cellType switch
            {
                CellTypeEnum.EmptyCell => EmptySprite,
                CellTypeEnum.WallCell => 0,
                CellTypeEnum.GroundCell => 1,
                CellTypeEnum.ShrineTopCell => 0,
                CellTypeEnum.ShrineMidCell => 0,
                CellTypeEnum.ShrineBotCell => 1,
                CellTypeEnum.PillarTopCell => 0,
                CellTypeEnum.PillarMidCell => 0,
                CellTypeEnum.PillarBotCell => 0,
                CellTypeEnum.DestructAbleWallCell => 1,
                _ => EmptySprite
            };
            return value;
        }

        private static int WallSkin()
        {
            return RandomNumberGenerator.GetInt32(0, 1000) switch
            {
                < WallPillarMiddleChance => PillarMiddleSpriteId,
                < WallBloodStainOnWallChance => OldBloodStainOnWallSpriteId,
                < WallSkullOnWallChance => SkullOnWallSpriteId,
                _ => 0
            };
        }

        private static int GroundSkin()
        {
            return RandomNumberGenerator.GetInt32(0, 1000) switch
            {
                < 5 => VerticalCrackSpriteId,
                < GroundSkullAndBoneChance => SkullAndBoneSpriteId,
                < GroundHorizontalCrackChance => HorizontalCrackSpriteId,
                < GroundOldBloodStainChance => OldBloodStainSpriteId,
                _ => 1
            };
        }

        internal int[,] MidGroundSpriteMatrix()
        {
            var result = new int[Height, Width];
            for (var i = 0; i < Height; i++)
                for (var j = 0; j < Width; j++)
                    result[i, j] = MidGroundSprite(CellGrid[i, j].CellType);

            return result;
        }

        private static int MidGroundSprite(CellTypeEnum cellType)
        {
            var value = cellType switch
            {
                CellTypeEnum.WallCell => WallSkin(),
                CellTypeEnum.GroundCell => GroundSkin(),
                CellTypeEnum.ShrineTopCell => BloodShrineTopId,
                CellTypeEnum.ShrineMidCell => BloodShrineTopSpriteId,
                CellTypeEnum.ShrineBotCell => BloodShrineBottomSpriteId,
                CellTypeEnum.PillarTopCell => PillarTopSpriteId,
                CellTypeEnum.PillarMidCell => PillarMiddleSpriteId,
                CellTypeEnum.PillarBotCell => PillarBottomSpriteId,
                CellTypeEnum.DestructAbleWallCell => DestructAbleWallPhase1SpriteId,
                CellTypeEnum.EmptyCell => EmptySprite,
                _ => EmptySprite
            };
            return value;
        }
    }
}