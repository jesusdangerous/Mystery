#nullable enable
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using MysteryWorld.Models;
using MysteryWorld.Models.Enums;
using MysteryWorld.Models.Interfaces;
using MysteryWorld.Views;

namespace MysteryWorld.Controllers;

public sealed class LevelController
{
    public int LevelCount { get; set; } = 1;
    public EventController EventDispatcher { get; set; } = null!;
    public CameraController Camera2d { get; private set; } = null!;
    internal SummonerController Summoner { get; private set; } = null!;
    public ArchenemyController? ArchEnemy { get; private set; }

    private Dictionary<string, SummonModel> MutableFriendlySummons { get; set; } = null!;
    private Dictionary<string, SummonModel> MutableHostileSummons { get; set; } = null!;
    public Dictionary<string, string> AttackerToTarget { get; private set; } = null!;
    public Dictionary<string, HashSet<string>> Attackers { get; private set; } = null!;
    private List<ProjectileView> MutableProjectiles { get; set; } = null!;
    private Dictionary<string, GameObjectView> MutableUsable { get; set; } = null!;

    public IReadOnlyDictionary<string, SummonModel> FriendlySummons => MutableFriendlySummons;
    public IReadOnlyDictionary<string, SummonModel> HostileSummons => MutableHostileSummons;
    public MapModel GameMap { get; private set; } = null!;
    public FogView FogOfWar { get; set; } = null!;
    public SafeGameObjectTreeModel QuadTree { get; private set; } = null!;
    public SafeGameObjectTreeModel MapTree { get; private set; } = null!;
    public BloodModel BloodShrine { get; private set; } = null!;
    public LadderModel? Ladder { get; private set; }
    public bool IsInTechnicalState { get; set; }
    public bool IsInAiState { get; set; }
    public int CurrentPoints;

    private LevelController()
    {
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        GameMap.Draw(spriteBatch, Camera2d.VisibleMap, FogOfWar, Camera2d.Zoom);
        Ladder?.Draw(spriteBatch);
        QuadTree.Draw(spriteBatch, Camera2d.VisibleArea);
        DrawSummonRange(spriteBatch);
        FogOfWar.Draw(spriteBatch, Camera2d.VisibleMap);
    }

    public void UpdateGameObjects(float deltaTime)
    {
        Camera2d.UpdateCamera();
        HandleSpellCoolDown(deltaTime);
        UpdateProjectiles(deltaTime);
        var toBeAdded = new List<GameObjectView>();
        foreach (var item in MutableUsable.Values)
        {
            item.Update(deltaTime);
            if (item.State == InstanceState.LimitReached)
            {
                if (item is TreasureChestModel)
                    toBeAdded.Add(new EnvironmentIemController(item.Position,
                        EnvironmentalAnimations.OpenChest, EnvironmentalMode.AnimationStop));
                MutableUsable.Remove(item.Id);
            }
        }

        foreach (var item in toBeAdded)
            MutableUsable.Add(item.Id, item);

        toBeAdded.Clear();
        ActionForCharacters(character =>
        {
            character.Update(deltaTime);
            HandleCharacterDeath(character);
        });

        Summoner.RegenerateMana();
        Summoner.CheckEnoughManaSpell();
    }

    public void UpdateQuadTree()
    {
        QuadTree.Clear();
        ActionForCharacters((character) => { QuadTree.Insert(character); });

        foreach (var potion in MutableUsable)
            QuadTree.Insert(potion.Value);

        foreach (var proj in MutableProjectiles)
            QuadTree.Insert(proj);
    }

    public void UpdateFog()
    {
        FogOfWar.Update(Summoner.Position, 6);
        foreach (var character in MutableFriendlySummons.Values)
            FogOfWar.Update(character.Position, 6);
    }

    private void FillMapTree()
    {
        for (var y = 0; y < GameMap.DungeonDimension; y++)
            for (var x = 0; x < GameMap.DungeonDimension; x++)
            {
                if (GameMap.Grid.GetCellType(x, y) == CellTypeEnum.WallCell)
                    MapTree.Insert(new WallModel(new Vector2(x * GameController.ScaledPixelSize + GameController.ScaledPixelSize / 2, y * GameController.ScaledPixelSize + GameController.ScaledPixelSize / 2), -1));
                if (GameMap.Grid.GetCellType(x, y) == CellTypeEnum.DestructAbleWallCell)
                    MapTree.Insert(new DestructWallModel(new Vector2(x * GameController.ScaledPixelSize + GameController.ScaledPixelSize / 2, y * GameController.ScaledPixelSize + GameController.ScaledPixelSize / 2), 2));
            }

        var bloodShrineTop = new EnvironmentIemController(BloodShrine.Position - new Vector2(0, 1) * GameController.ScaledPixelSize, EnvironmentalAnimations.BloodFountainTop, EnvironmentalMode.FullAnimation);
        var bloodShrineBottom = new EnvironmentIemController(BloodShrine.Position, EnvironmentalAnimations.BloodFountainBottom, EnvironmentalMode.FullAnimation);
        var shrineHint = new EnvironmentIemController(BloodShrine.Position + new Vector2(0, 1) * GameController.ScaledPixelSize, EnvironmentalAnimations.ShrineHintMap, EnvironmentalMode.SingleSprite);
        AddToMutableUseAble(bloodShrineTop);
        AddToMutableUseAble(bloodShrineBottom);
        AddToMutableUseAble(shrineHint);
        MapTree.Insert(BloodShrine);
    }

    public bool IsSummonLimitReached() =>
        MutableFriendlySummons.Count > 4;

    public bool IsSpaceFree(Vector2 position) =>
        QuadTree.PointSearchCharacters(position).Count <= 0;

    public bool OnGround(Vector2 position)
    {
        var mapPos = position.ToGrid();
        return !GameMap.Collidable[(int)mapPos.Y, (int)mapPos.X];
    }

    private void HandleCharacterDeath(CharacterController character)
    {
        if (character.CurrentLifePoints > 0) return;

        if (AttackerToTarget.ContainsKey(character.Id))
        {
            var targetId = AttackerToTarget[character.Id];
            Attackers[targetId].Remove(character.Id);
            if (Attackers[AttackerToTarget[character.Id]].Count == 0)
            {
                Attackers.Remove(targetId);
                var target = GetCharacterWithId(targetId);
                if (target is { IsFriendly: false })
                    target.Selected = false;
            }

            AttackerToTarget.Remove(character.Id);
        }

        if (Attackers.ContainsKey(character.Id))
        {
            foreach (var attackerId in Attackers[character.Id])
            {
                AttackerToTarget.Remove(attackerId);

                var attacker = GetCharacterWithId(attackerId);
                if (attacker is { IsFriendly: true })
                    attacker.CurrentState = CharacterState.PlayerControl;
            }

            Attackers.Remove(character.Id);
        }

        if (character is SummonModel summon)
        {
            if (summon.IsFriendly)
                MutableFriendlySummons.Remove(summon.Id);
            else
            {
                CurrentPoints += 10;

                var newItem = summon.DropItem();
                if (newItem != null)
                    MutableUsable.Add(newItem.Id, newItem);
                MutableHostileSummons.Remove(summon.Id);
            }

            var testEnvironItem = new EnvironmentIemController(summon.Position, EnvironmentalAnimations.BloodStain, EnvironmentalMode.SingleSprite);
            MutableUsable.Add(testEnvironItem.Id, testEnvironItem);
            return;
        }

        if (character.Id == Summoner.Id)
        {
            LoseGame();
            return;
        }

        if (ArchEnemy != null)
        {
            CurrentPoints += 100;

            LevelCount++;
            if (LevelCount < 6)
            {
                EventDispatcher.SendPopupEvent(new IPopupEvent.NotificationPopup("Hmmm!", Color.Red));
                Ladder = new LadderModel(CameraController.TileCenterToWorld(ArchEnemy.Position.ToGrid()));
                ArchEnemy = null;
                return;
            }
            WinGame();
        }
        else ArchEnemy = null;
    }


    private void LoseGame()
    {
        EventDispatcher.SendScreenRequest(new INavigationEvent.PopScreen());
        EventDispatcher.SendScreenRequest(new INavigationEvent.GameOverScreen());
        if (IsInTechnicalState) return;
        EventDispatcher.SendPopupEvent(new IPopupEvent.NotificationPopup("Points: " + CurrentPoints, Color.Red));
    }

    private void WinGame()
    {
        EventDispatcher.SendScreenRequest(new INavigationEvent.PopScreen());
        EventDispatcher.SendScreenRequest(new INavigationEvent.GameWonScreen());
        if (IsInTechnicalState) return;
        EventDispatcher.SendPopupEvent(new IPopupEvent.NotificationPopup("Points: " + CurrentPoints, Color.Red));
    }

    public void ActionForCharacters(Action<CharacterController> action)
    {
        ActionForFriendlyCharacters(action);
        ActionForHostileCharacters(action);
    }

    public void ActionForFriendlyCharacters(Action<CharacterController> action)
    {
        action(Summoner);
        MutableFriendlySummons.Values.ForEachReversed(action);
    }

    private void ActionForHostileCharacters(Action<CharacterController> action)
    {
        if (ArchEnemy != null)
            action(ArchEnemy);
        MutableHostileSummons.Values.ForEachReversed(action);
    }

    public void AddToMutableUseAble(GameObjectView newItem)
    {
        MutableUsable.Add(newItem.Id, newItem);
    }

    public void AddToMutableUseAble(List<GameObjectView> content)
    {
        foreach (var item in content)
            MutableUsable.Add(item.Id, item);
    }

    private void UpdateProjectiles(float deltaTime)
    {
        MutableProjectiles.ForEachReversed(projectile =>
            {
                projectile.Update(deltaTime);
                if (projectile.State == InstanceState.LimitReached)
                    MutableProjectiles.Remove(projectile);

                var collidingWalls = MapTree.Search(projectile.Hitbox).OfType<WallModel>().ToList();
                foreach (var wall in collidingWalls)
                {
                    if (GameMap.DungeonMidGround[(int)wall.Position.ToGrid().Y, (int)wall.Position.ToGrid().X] == 14)
                    {
                        GameMap.DungeonMidGround[(int)wall.Position.ToGrid().Y, (int)wall.Position.ToGrid().X] = 15;
                        continue;
                    }

                    if (wall is DestructWallModel && projectile.Damage > 0)
                    {
                        if (wall.HitsLeft-- == 0)
                        {
                            GameMap.Grid.SetCell((int)wall.Position.ToGrid().X,
                                (int)wall.Position.ToGrid().Y,
                                CellTypeEnum.GroundCell);
                            GameMap.DungeonMidGround[(int)wall.Position.ToGrid().Y,
                                (int)wall.Position.ToGrid().X] = 20;
                            GameMap.DungeonBackGround[(int)wall.Position.ToGrid().Y,
                                (int)wall.Position.ToGrid().X] = 1;
                            GameMap.Collidable[(int)wall.Position.ToGrid().Y, (int)wall.Position.ToGrid().X] =
                                false;
                            MapTree.Clear();
                            FillMapTree();
                        }
                        else
                        {
                            wall.HitsLeft--;
                            GameMap.DungeonMidGround[(int)wall.Position.ToGrid().Y,
                                (int)wall.Position.ToGrid().X] = 19;
                        }
                        continue;
                    }
                }
                if (collidingWalls.Count > 0)
                    RemoveProjectile(projectile);
            }
        );
    }

    public void AddProjectile(ProjectileView projectile)
    {
        MutableProjectiles.Add(projectile);
    }

    public void ConsumeItem(ItemModel item)
    {
        item.Use(Summoner);
        MutableUsable.Remove(item.Id);
    }

    public void PickUpSoul(ItemModel soul)
    {
        CurrentPoints += 1;
        Summoner.Souls += 1;
        MutableUsable.Remove(soul.Id);
    }

    private void RemoveProjectile(ProjectileView projectile)
    {
        MutableProjectiles.Remove(projectile);
    }

    public void AddFriendlySummon(SummonModel summon)
    {
        MutableFriendlySummons.Add(summon.Id, summon);
    }

    public void AddHostileSummon(SummonModel summon)
    {
        MutableHostileSummons.Add(summon.Id, summon);
    }

    public void AddItem(ItemModel item)
    {
        MutableUsable.Add(item.Id, item);
    }

    public SummonModel? GetSummonWithId(string id) =>
        MutableFriendlySummons!.GetValueOrDefault(id, MutableHostileSummons!.GetValueOrDefault(id, null));

    public CharacterController? GetCharacterWithId(string id)
    {
        var summon = GetSummonWithId(id);
        if (summon != null) return summon;
        if (Summoner.Id == id) return Summoner;
        return ArchEnemy?.Id == id ? ArchEnemy : null;
    }

    public CharacterController? GetEnemyWithId(string id)
    {
        var summon = MutableHostileSummons!.GetValueOrDefault(id, null);
        if (summon != null)
            return summon;
        return ArchEnemy?.Id == id ? ArchEnemy : null;
    }

    public void ChangeLevel()
    {
        CurrentPoints += 200;

        AttackerToTarget.Clear();
        Attackers.Clear();
        QuadTree.Clear();
        MapTree.Clear();
        GameMap.GenerateNewMap();
        FogOfWar = FogView.CreateFogOfWar(GameMap, true);
        MapTree = SafeGameObjectTreeModel.CreateQuadTree(rect: new Rect(new Vector2(0, 0), size: new Vector2((GameMap.DungeonDimension + 10) * GameController.ScaledPixelSize,
                    y: (GameMap.DungeonDimension + 10) * GameController.ScaledPixelSize)));
        FillMapTree();
        var spawnPoint = GameMap.GetSpawnPoint();
        Summoner.SetPosition(spawnPoint);
        Ladder = null;
        BloodShrine.Position = new Vector2(spawnPoint.X, spawnPoint.Y - GameController.ScaledPixelSize);
        Camera2d.SetCameraPosition(spawnPoint);
        ArchEnemy = new ArchenemyController(GameMap.GetBossSpawnPoint(), LevelCount, null);
        var horizontalOffset = -2 * GameController.ScaledPixelSize;
        foreach (var friendly in MutableFriendlySummons)
        {
            friendly.Value.SetPosition(spawnPoint + new Vector2(horizontalOffset, GameController.ScaledPixelSize));
            horizontalOffset += GameController.ScaledPixelSize;
        }

        MutableHostileSummons = ObjectController.PopulateDungeon(GameMap.RoomList,
            new List<RoomTypeEnum>()
            {
                RoomTypeEnum.PillarRoom, RoomTypeEnum.EmptyRoom, RoomTypeEnum.EmptyRoom, RoomTypeEnum.PillarRoom,
                RoomTypeEnum.LabyrinthRoom, RoomTypeEnum.LayerRoom
            },
            LevelCount);
        MutableUsable = ObjectController.SpawnLoot(GameMap.RoomList, LevelCount);
        var bloodShrineTop =
            new EnvironmentIemController(BloodShrine.Position - new Vector2(0, 1) * GameController.ScaledPixelSize, EnvironmentalAnimations.BloodFountainTop, EnvironmentalMode.FullAnimation);
        var bloodShrineBottom =
            new EnvironmentIemController(BloodShrine.Position, EnvironmentalAnimations.BloodFountainBottom, EnvironmentalMode.FullAnimation);
        MutableUsable.Add(bloodShrineTop.Id, bloodShrineTop);
        MutableUsable.Add(bloodShrineBottom.Id, bloodShrineBottom);
        MutableProjectiles.Clear();
        EventDispatcher.SendPopupEvent(new IPopupEvent.NotificationPopup("Level " + LevelCount + "/5", Color.White, 2f));
    }

    public static LevelController CreateDefaultLevelState()
    {
        var map = MapModel.CreateMap();
        var fog = FogView.CreateFogOfWar(map, true);
        var spawnPoint = map.GetSpawnPoint();
        var bossPoint = map.GetBossSpawnPoint();

        var levelState = new LevelController
        {
            Camera2d = new CameraController(new Vector2(spawnPoint.X - 2 * GameController.ScaledPixelSize, spawnPoint.Y + GameController.ScaledPixelSize)),
            MutableFriendlySummons = new Dictionary<string, SummonModel>(),
            BloodShrine = new BloodModel(new Vector2(spawnPoint.X, spawnPoint.Y - GameController.ScaledPixelSize)),
            Ladder = null,
            MutableHostileSummons = ObjectController.PopulateDungeon(map.RoomList, new List<RoomTypeEnum>() { RoomTypeEnum.PillarRoom, RoomTypeEnum.EmptyRoom, RoomTypeEnum.EmptyRoom, RoomTypeEnum.PillarRoom, RoomTypeEnum.LabyrinthRoom, RoomTypeEnum.LayerRoom }, 1),
            MutableUsable = ObjectController.CreateDefaultItems(map.RoomList),
            Summoner = new SummonerController(new Vector2(spawnPoint.X - 2 * GameController.ScaledPixelSize, spawnPoint.Y + GameController.ScaledPixelSize)),
            ArchEnemy = new ArchenemyController(bossPoint, 1, null),
            MutableProjectiles = new List<ProjectileView>(),
            GameMap = map,
            FogOfWar = fog,
            QuadTree = SafeGameObjectTreeModel.CreateQuadTree(
                rect: new Rect(new Vector2(-1000, -1000),
                    size: new Vector2((map.DungeonDimension + 10) * GameController.ScaledPixelSize,
                        y: (map.DungeonDimension + 10) * GameController.ScaledPixelSize))
            ),
            MapTree = SafeGameObjectTreeModel.CreateQuadTree(
                rect: new Rect(new Vector2(0, 0),
                    size: new Vector2((map.DungeonDimension + 10) * GameController.ScaledPixelSize,
                        y: (map.DungeonDimension + 10) * GameController.ScaledPixelSize))
            ),
            Attackers = new Dictionary<string, HashSet<string>>(),
            AttackerToTarget = new Dictionary<string, string>(),
            IsInTechnicalState = false,
            IsInAiState = false
        };
        levelState.FillMapTree();
        return levelState;
    }

    public static LevelController CreateTechDemoLevelState()
    {
        var map = MapModel.TechDemoMap();
        var fog = FogView.CreateFogOfWar(map, false);
        var spawnPoint = map.GetSpawnPoint();
        var bossPoint = new Vector2(map.BossPosition.X * GameController.ScaledPixelSize + GameController.ScaledPixelSize / 2f,
            (map.BossPosition.Y + 5) * GameController.ScaledPixelSize + GameController.ScaledPixelSize / 2f);

        var levelState = new LevelController
        {
            Camera2d = new CameraController(spawnPoint),
            MutableHostileSummons = ObjectController.CreateTechDemoHostiles(map.RoomTopLeftCornerList[1]),
            MutableFriendlySummons = ObjectController.CreateTechDemoFriendlies(map.RoomTopLeftCornerList[0]),
            MutableUsable = new Dictionary<string, GameObjectView>(),
            Summoner = new SummonerController(spawnPoint),
            BloodShrine = new BloodModel(new Vector2(spawnPoint.X - 16 * GameController.ScaledPixelSize, spawnPoint.Y)),
            Ladder = null, ArchEnemy = new ArchenemyController(bossPoint, 1, null), MutableProjectiles = new List<ProjectileView>(),
            GameMap = map, FogOfWar = fog,
            QuadTree = SafeGameObjectTreeModel.CreateQuadTree( rect: new Rect(new Vector2(0, 0), size: new Vector2((map.DungeonDimension + 10) * GameController.ScaledPixelSize,
                        y: (map.DungeonDimension + 10) * GameController.ScaledPixelSize))),
            MapTree = SafeGameObjectTreeModel.CreateQuadTree(rect: new Rect(new Vector2(0, 0), size: new Vector2((map.DungeonDimension + 10) * GameController.ScaledPixelSize,
                        y: (map.DungeonDimension + 10) * GameController.ScaledPixelSize))),
            Attackers = new Dictionary<string, HashSet<string>>(),
            AttackerToTarget = new Dictionary<string, string>(),
            IsInTechnicalState = true,
            IsInAiState = false
        };
        levelState.Summoner.Souls = 10000;
        levelState.FillMapTree();
        return levelState;
    }

    public static LevelController CreateAiTestLevelState(EnemyType testEnemy, int level)
    {
        var map = MapModel.AiMap();
        var fog = FogView.CreateFogOfWar(map, false);
        var spawnPoint = map.GetSpawnPoint();
        var bossPoint = map.GetBossSpawnPoint();
        var enemy = new List<SummonModel>();
        var archEnemy = new ArchenemyController(bossPoint, 5, null);
        if (testEnemy is not EnemyType.ArchEnemy)
        {
            enemy.Add(GetEnemyByType(testEnemy, level, bossPoint));
            archEnemy = new ArchenemyController(bossPoint + new Vector2(2, 0) * GameController.ScaledPixelSize, level, new NoneBehaviourModel());
        }

        var levelState = new LevelController
        {
            Camera2d = new CameraController(spawnPoint),
            MutableFriendlySummons = new Dictionary<string, SummonModel>(),
            MutableHostileSummons = enemy.ToDictionary(summon => summon.Id, summon => summon),
            MutableUsable = new Dictionary<string, GameObjectView>(),
            Summoner = new SummonerController(spawnPoint),
            BloodShrine = new BloodModel(new Vector2((map.RoomTopLeftCornerList[0].X + 1.5f) * GameController.ScaledPixelSize, (map.RoomTopLeftCornerList[0].Y + 7.5f) * GameController.ScaledPixelSize)),
            Ladder = null, ArchEnemy = archEnemy, MutableProjectiles = new List<ProjectileView>(), GameMap = map, FogOfWar = fog,
            QuadTree = SafeGameObjectTreeModel.CreateQuadTree(rect: new Rect(new Vector2(0, 0), size: new Vector2((map.DungeonDimension + 10) * GameController.ScaledPixelSize,
                        y: (map.DungeonDimension + 10) * GameController.ScaledPixelSize))),
            MapTree = SafeGameObjectTreeModel.CreateQuadTree(rect: new Rect(new Vector2(0, 0),size: new Vector2((map.DungeonDimension + 10) * GameController.ScaledPixelSize,
                        y: (map.DungeonDimension + 10) * GameController.ScaledPixelSize))),
            Attackers = new Dictionary<string, HashSet<string>>(), AttackerToTarget = new Dictionary<string, string>(),
            IsInTechnicalState = true, IsInAiState = true
        };
        levelState.Summoner.Souls = 10000;
        levelState.Summoner.SkillPoints = 20;
        levelState.FillMapTree();
        return levelState;
    }

    private static SummonModel GetEnemyByType(EnemyType enemyType, int level, Vector2 position)
    {
        switch (enemyType)
        {
            case EnemyType.Paladin:
                return new PaladinController(position, level);
            default:
                throw new ArgumentOutOfRangeException(nameof(enemyType), enemyType, null);
        }
    }

    private void DrawSummonRange(SpriteBatch spriteBatch)
    {
        if (Summoner.SelectedSummonType == null) return;
        for (var y = (int)-Summoner.SummonRange; y <= (int)Summoner.SummonRange; y++)
            for (var x = (int)-Summoner.SummonRange; x <= (int)Summoner.SummonRange; x++)
                if (GameMap.DungeonBackGround[(int)Summoner.Position.ToGrid().Y + y, (int)Summoner.Position.ToGrid().X + x] == 1)
                {
                    spriteBatch.Draw(AssetController.SpriteSheet,
                        new Vector2((int)Summoner.Position.ToGrid().X + x + 0.5f, (int)Summoner.Position.ToGrid().Y + y + 0.5f) * GameController.ScaledPixelSize,
                        Summoner.IsCanSummon() && MutableFriendlySummons.Count < 5 ? AssetController.GetRectangle(61) : AssetController.GetRectangle(62),
                        Color.White * 0.2f, 0f, GameController.Origin, GameController.Scale, SpriteEffects.None, 0.8f);
                }
    }

    private void HandleSpellCoolDown(float deltaTime)
    {
        if (Summoner.FireBallCoolDown < SummonerController.CoolDownLimitFire)
            Summoner.FireBallCoolDown += deltaTime;
        if (Summoner.HealingSpellCoolDown < SummonerController.CoolDownLimitHeal)
            Summoner.HealingSpellCoolDown += deltaTime;
        if (Summoner.SpeedSpellCoolDown < SummonerController.CoolDownLimitSpeed)
            Summoner.SpeedSpellCoolDown += deltaTime;
    }

    public void RefreshQuadTree()
    {
        QuadTree = SafeGameObjectTreeModel.CreateQuadTree(rect: new Rect(new Vector2(-1000, -1000), size: new Vector2((GameMap.DungeonDimension + 10) * GameController.ScaledPixelSize,
                    y: (GameMap.DungeonDimension + 10) * GameController.ScaledPixelSize)));
        MapTree = SafeGameObjectTreeModel.CreateQuadTree( rect: new Rect(new Vector2(0, 0), size: new Vector2((GameMap.DungeonDimension + 10) * GameController.ScaledPixelSize,
                    y: (GameMap.DungeonDimension + 10) * GameController.ScaledPixelSize)));
        UpdateQuadTree();
        FillMapTree();
    }
}