using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using MysteryWorld.Views;
using MysteryWorld.Controllers;
using MysteryWorld.Models.Interfaces;

namespace MysteryWorld.Models
{
    public sealed class MapModel : IGraph
    {
        public int[,] DungeonBackGround { get; private set; }
        public int[,] DungeonMidGround { get; private set; }
        public int RoomCount { get; private set; }
        public List<RoomModel> RoomList { get; private set; }
        public List<Vector2> RoomTopLeftCornerList { get; private set; }
        public int DungeonDimension { get; private set; }
        public bool[,] Collidable { get; private set; }
        public Vector2 SpawnPosition { get; private set; }
        public Vector2 BossPosition { get; private set; }
        public GridController Grid { get; private set; }

        private const float F1 = 0.1f;
        private const float F2 = 2f;
        private const float F10 = 10f;

        private MapModel()
        {
        }

        public static MapModel CreateMap(int stageNumber = 1)
        {
            var map = new MapModel();
            map.GenerateNewMap(stageNumber);
            return map;
        }

        public void GenerateNewMap(int stageNumber = 1)
        {
            RoomCount = 7 + 5 * stageNumber;
            DungeonDimension = 100 + 20 * stageNumber;
            var dungeon = new DungeonController(RoomCount, DungeonDimension, DungeonDimension, 12,
                DungeonDimension - 12, 12, DungeonDimension - 12, 19, 15, 55);
            RoomList = dungeon.Rooms;
            RoomTopLeftCornerList = new List<Vector2>();
            foreach (var room in RoomList)
                RoomTopLeftCornerList.Add(new Vector2((float)room.TopLeftCorner.X, (float)room.TopLeftCorner.Y));

            SpawnPosition = new Vector2((int)dungeon.Rooms[0].Middle.X, (int)dungeon.Rooms[0].Middle.Y - 3);
            BossPosition = new Vector2((int)dungeon.Rooms[1].Middle.X, (int)dungeon.Rooms[1].Middle.Y);
            DungeonBackGround = dungeon.Tiles.BackgroundSpriteMatrix();
            DungeonMidGround = dungeon.Tiles.MidGroundSpriteMatrix();
            Collidable = dungeon.Tiles.CollisionMatrix();
            Grid = dungeon.Tiles;
        }

        public static MapModel TechDemoMap()
        {
            var map = new MapModel
            {
                RoomCount = 3,
                DungeonDimension = 200
            };
            var dungeon = new DungeonController();
            dungeon = DungeonController.CreateTechDemoDungeon();
            map.RoomList = dungeon.Rooms;
            map.RoomTopLeftCornerList = new List<Vector2>();
            foreach (var room in map.RoomList)
                map.RoomTopLeftCornerList.Add(new Vector2((float)room.TopLeftCorner.X, (float)room.TopLeftCorner.Y));

            map.SpawnPosition = new Vector2(85, 100);
            map.BossPosition = new Vector2(95, 100);
            map.DungeonBackGround = dungeon.Tiles.BackgroundSpriteMatrix();
            map.DungeonMidGround = dungeon.Tiles.MidGroundSpriteMatrix();
            map.Collidable = dungeon.Tiles.CollisionMatrix();
            map.Grid = dungeon.Tiles;
            return map;
        }

        public static MapModel AiMap()
        {
            var map = new MapModel
            {
                RoomCount = 1,
                DungeonDimension = 50
            };
            var dungeon = new DungeonController();
            dungeon = DungeonController.CreateAiDungeon();
            map.RoomList = dungeon.Rooms;
            map.RoomTopLeftCornerList = new List<Vector2>() { new((float)map.RoomList[0].TopLeftCorner.X, (float)map.RoomList[0].TopLeftCorner.Y) };
            map.SpawnPosition = new Vector2(15, 25);
            map.BossPosition = new Vector2(35, 25);
            map.DungeonBackGround = dungeon.Tiles.BackgroundSpriteMatrix();
            map.DungeonMidGround = dungeon.Tiles.MidGroundSpriteMatrix();
            map.Collidable = dungeon.Tiles.CollisionMatrix();
            map.Grid = dungeon.Tiles;
            return map;
        }

        public Vector2 GetSpawnPoint() =>
            new(SpawnPosition.X * GameController.ScaledPixelSize + GameController.ScaledPixelSize / F2,
                (SpawnPosition.Y + 5) * GameController.ScaledPixelSize + GameController.ScaledPixelSize / F2);

        public Vector2 GetBossSpawnPoint() =>
            new(BossPosition.X * GameController.ScaledPixelSize + GameController.ScaledPixelSize / F2,
                BossPosition.Y * GameController.ScaledPixelSize + GameController.ScaledPixelSize / F2);

        public void Draw(SpriteBatch spriteBatch, Rectangle visibleMapArea, FogView fog, float zoom)
        {
            for (var y = Math.Max(0, visibleMapArea.Top); y < Math.Min(DungeonDimension - 1, visibleMapArea.Bottom); y++)
                for (var x = Math.Max(0, visibleMapArea.Left); x < Math.Min(DungeonDimension - 1, visibleMapArea.Right); x++)
                {
                    if (DungeonMidGround[y, x] != 31 && (!fog.FogMask[x, y] || GameController.DebugMode || !fog.Use))
                    {
                        spriteBatch.Draw(AssetController.SpriteSheet,
                            new Vector2(GameController.ScaledPixelSize * x, GameController.ScaledPixelSize * y), AssetController.GetRectangle(DungeonMidGround[y, x]),
                            Color.White, 0f, Vector2.Zero, GameController.Scale, SpriteEffects.None, 1f);
                    }

                    if (GameController.DebugMode && DungeonBackGround[y, x] != 31)
                    {
                        spriteBatch.DrawRectangle(new Rectangle((int)(GameController.ScaledPixelSize * x),
                                (int)(GameController.ScaledPixelSize * y),
                                (int)GameController.ScaledPixelSize,
                                (int)GameController.ScaledPixelSize),
                            (int)(2 / zoom) + 1, Color.Black, F1);
                    }
                }
        }

        private static readonly Vector2[] sDirections = new Vector2[]
        {
            new(0, 1),
            new(0, -1),
            new(-1, 0),
            new(1, 0),
            new(1,1),
            new(-1,-1),
            new(-1,1),
            new(1,-1),
        };

        public bool Passable(Vector2 id)
        {
            if (id.X < 0 || id.X >= DungeonDimension - 1 || id.Y < 0 || id.Y >= DungeonDimension) return false;
            return !Collidable[(int)id.Y, (int)id.X];
        }

        public double Cost(Vector2 a, Vector2 b) =>
            Math.Round(Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2)) * 10) / F10;

        public IEnumerable<Vector2> PassableNeighbors(Vector2 id)
        {
            foreach (Vector2 direction in sDirections)
            {
                var next = new Vector2(id.X + direction.X, id.Y + direction.Y);
                if (Passable(next))
                    yield return next;
            }
        }
    }
}