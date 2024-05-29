using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System;
using MysteryWorld.Models;
using MysteryWorld.Views;
using MysteryWorld.Models.Enums;

namespace MysteryWorld.Controllers;

internal static class ObjectController
{
    private const int PerHundred = 100;
    private const int PerThousand = 1000;
    private const float HalfUnit = 2f;

    private const int EmptyRoomSummonLimit = 7;
    private const int RoomHeight = 15;
    private const int RoomWidth = 19;

    private const int ChestSpawnChance = 40;
    private const int TechEnemy = 20;
    private const int TechX = 25;
    private const int TechFriendly = 40;


    internal static Dictionary<string, GameObjectView> CreateDefaultItems(List<RoomModel> roomList)
    {
        var random = new Random((int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds % int.MaxValue));
        var result = new List<GameObjectView>();
        var referencePoint = new Vector2((float)((roomList[0].Middle.X + 0.5f) * GameController.ScaledPixelSize), (float)((roomList[0].Middle.Y + 0.5f) * GameController.ScaledPixelSize));
        for (var i = 2; i < roomList.Count; i++)
            if (random.Next() % PerHundred < ChestSpawnChance)
                result.Add(new TreasureChestModel(CameraController.TileCenterToWorld(new Vector2((int)roomList[i].TopLeftCorner.X + 10, (int)roomList[i].TopLeftCorner.Y + 10))));

        result.Add(new HealthPotionModel(referencePoint + new Vector2(0, 4) * GameController.ScaledPixelSize));
        result.Add(new TreasureChestModel(referencePoint + new Vector2(0, 6) * GameController.ScaledPixelSize));

        result.Add(new TreasureChestModel(CameraController.TileCenterToWorld(new Vector2((int)roomList[1].TopLeftCorner.X + 17, (int)roomList[1].TopLeftCorner.Y + 14))));
        result.Add(new TreasureChestModel(CameraController.TileCenterToWorld(new Vector2((int)roomList[1].TopLeftCorner.X + 1, (int)roomList[1].TopLeftCorner.Y + 14))));
        result.Add(new TreasureChestModel(CameraController.TileCenterToWorld(new Vector2((int)roomList[1].TopLeftCorner.X + 17, (int)roomList[1].TopLeftCorner.Y))));
        result.Add(new TreasureChestModel(CameraController.TileCenterToWorld(new Vector2((int)roomList[1].TopLeftCorner.X + 1, (int)roomList[1].TopLeftCorner.Y))));

        return result.ToDictionary(summon => summon.Id, summon => summon);
    }

    internal static Dictionary<string, SummonModel> CreateTechDemoFriendlies(Vector2 roomPos)
    {
        var result = new List<SummonModel>();
        for (var y = 0; y < TechFriendly; y++)
            for (var x = 0; x < TechX; x++)
            {
                SummonModel enemy = new PaladinController(new Vector2((roomPos.X + 2 + x) * GameController.ScaledPixelSize + GameController.ScaledPixelSize / HalfUnit,
                    (roomPos.Y + 1 + y) * GameController.ScaledPixelSize + GameController.ScaledPixelSize / HalfUnit), 1);
                result.Add(enemy);
            }
        return result.ToDictionary(summon => summon.Id, summon => summon);
    }

    internal static Dictionary<string, SummonModel> CreateTechDemoHostiles(Vector2 roomPos)
    {
        var result = new List<SummonModel>();
        for (var y = 0; y < TechEnemy; y++)
            for (var x = 0; x < TechX; x++)
            {
                SummonModel enemy = new PaladinController(new Vector2((roomPos.X + 2 + x) * GameController.ScaledPixelSize + GameController.ScaledPixelSize / HalfUnit,
                    (roomPos.Y + 1 + y) * GameController.ScaledPixelSize + GameController.ScaledPixelSize / HalfUnit), 2);
                result.Add(enemy);
            }
        return result.ToDictionary(summon => summon.Id, summon => summon);
    }

    internal static Dictionary<string, SummonModel> PopulateDungeon(List<RoomModel> roomList, List<RoomTypeEnum> typeList, int stage)
    {
        var random = new Random((int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds % int.MaxValue));
        var result = new List<SummonModel>();
        result.AddRange(PopulateBossRoom(random, new Vector2((float)roomList[1].TopLeftCorner.X, (float)roomList[1].TopLeftCorner.Y), stage));
        for (var i = 2; i < roomList.Count; i++)
        {
            var current = PopulateRoom(random, new Vector2((float)roomList[i].TopLeftCorner.X, (float)roomList[i].TopLeftCorner.Y), typeList[i % typeList.Count], stage);
            result.AddRange(current);
        }
        return result.ToDictionary(summon => summon.Id, summon => summon);
    }

    private static List<SummonModel> PopulateRoom(Random rand, Vector2 tlc, RoomTypeEnum roomType, int stage)
    {
        var result = new List<SummonModel>();
        switch (roomType)
        {
            case RoomTypeEnum.EmptyRoom:
                result = PopulateEmptyRoom(rand, tlc, stage);
                break;
            case RoomTypeEnum.LabyrinthRoom:
                result = PopulateLabyrinthRoom(rand, tlc, stage);
                break;
            case RoomTypeEnum.LayerRoom:
                result = PopulateLayerRoom(rand, tlc, stage);
                break;
            case RoomTypeEnum.PillarRoom:
                result = PopulatePillarRoom(rand, tlc, stage);
                break;
        }

        return result;
    }

    private static List<SummonModel> PopulateEmptyRoom(Random rand, Vector2 tlc, int stage)
    {
        var result = new List<SummonModel>();
        var current = 0;
        for (var i = 0; i < (RoomHeight - 1) * (RoomWidth - 1); i++)
        {
            if (current > EmptyRoomSummonLimit) break;
            if (rand.Next() % PerThousand < 33 - 2 * current)
            {
                current++;
                switch (rand.Next() % 5)
                {
                    case 2:
                        result.Add(new PaladinController(CameraController.TileCenterToWorld(new Vector2(tlc.X + i % RoomWidth, tlc.Y + (int)((float)i / RoomWidth))), stage));
                        break;
                }
            }

        }
        return result;
    }

    private static List<SummonModel> PopulateLabyrinthRoom(Random rand, Vector2 tlc, int stage)
    {
        return new List<SummonModel>
        {
            new PaladinController(CameraController.TileCenterToWorld(new Vector2(tlc.X + 17, tlc.Y + 13)), stage),
            new PaladinController(CameraController.TileCenterToWorld(new Vector2(tlc.X + 13, tlc.Y + 3)), stage),
            new PaladinController(CameraController.TileCenterToWorld(new Vector2(tlc.X + 14, tlc.Y + 3)), stage)
        };
    }

    private static List<SummonModel> PopulateLayerRoom(Random rand, Vector2 tlc, int stage)
    {
        return new List<SummonModel>
        {
            new PaladinController(CameraController.TileCenterToWorld(new Vector2(tlc.X + 13, tlc.Y + 3)), stage),
            new PaladinController(CameraController.TileCenterToWorld(new Vector2(tlc.X + 14, tlc.Y + 3)), stage)
        };
    }

    private static List<SummonModel> PopulatePillarRoom(Random rand, Vector2 tlc, int stage)
    {
        return new List<SummonModel>
        {
            new PaladinController(CameraController.TileCenterToWorld(new Vector2(tlc.X + 6, tlc.Y + 7)), stage),
            new PaladinController(CameraController.TileCenterToWorld(new Vector2(tlc.X + 13, tlc.Y + 3)), stage),
            new PaladinController(CameraController.TileCenterToWorld(new Vector2(tlc.X + 14, tlc.Y + 3)), stage)
        };
    }

    private static List<SummonModel> PopulateBossRoom(Random rand, Vector2 tlc, int stage)
    {
        var result = new List<SummonModel>();
        for (var i = 0; i < 2; i++)
        {
            switch (rand.Next() % 5)
            {
                case 2:
                    result.Add(new PaladinController(CameraController.TileCenterToWorld(new Vector2(tlc.X + 2, tlc.Y + 2 + 2 * i)), stage));
                    break;
            }
        }
        return result;
    }

    internal static Dictionary<string, GameObjectView> SpawnLoot(List<RoomModel> roomList, int stage)
    {
        var random = new Random((int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds % int.MaxValue));
        var result = new List<GameObjectView>();

        result.AddRange(BossRoomLoot(new Vector2((int)roomList[1].TopLeftCorner.X, (int)roomList[1].TopLeftCorner.Y), stage));
        for (var i = 2; i < roomList.Count; i++)
            if (random.Next() % PerHundred < ChestSpawnChance)
                result.Add(new TreasureChestModel(CameraController.TileCenterToWorld(new Vector2((int)roomList[i].TopLeftCorner.X + 10, (int)roomList[i].TopLeftCorner.Y + 10))));

        return result.ToDictionary(summon => summon.Id, summon => summon);
    }

    private static List<GameObjectView> BossRoomLoot(Vector2 tlc, int stage)
    {
        var result = new List<GameObjectView>();
        if (stage <= 0) return result;
        result.Add(new TreasureChestModel(CameraController.TileCenterToWorld(tlc + new Vector2(1, 0))));
        return result;
    }
}