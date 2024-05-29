using System;
using System.Collections.Generic;
using System.Linq;
using MysteryWorld.Models;
using MysteryWorld.Models.Enums;

namespace MysteryWorld.Controllers
{
    public sealed class DungeonController
    {
        private GraphModel mesh;
        public GridController Tiles;
        internal List<RoomModel> Rooms;
        private List<FloorModel> horizontalFloors;
        private List<FloorModel> verticalFloors;
        private Random Random;

        public DungeonController()
        {
        }

        public DungeonController(int numberrooms, int dungeonx, int dungeony, int xmin, int xmax, int ymin, int ymax, int roomwidth, int roomheight, int addpercent)
        {
            Rooms = new List<RoomModel>();
            horizontalFloors = new List<FloorModel>();
            verticalFloors = new List<FloorModel>();
            Random = new Random((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds % int.MaxValue);
            CreateRoomList(numberrooms, xmin, xmax, ymin, ymax, roomwidth, roomheight, Random);
            CreateMesh(addpercent);
            CreateFloors();
            Tiles = new GridController(dungeonx, dungeony);
            FillGround();
            FillWalls();
            Interior();
        }

        public static DungeonController CreateTechDemoDungeon()
        {
            var dungeon = new DungeonController
            {
                Rooms = new List<RoomModel>() { CreateRoomAt(30, 100, 45, 45), CreateRoomAt(150, 100, 45, 45), CreateRoomAt(90, 100, 45, 45), CreateRoomAt(90, 140, 25, 45) },
                horizontalFloors = new List<FloorModel>() { new(new PointModel(30, 100), new PointModel(90, 100)), new(new PointModel(100, 100), new PointModel(150, 170)), new(new PointModel(90, 140), new PointModel(30, 140)), new(new PointModel(90, 140), new PointModel(150, 140)) },
                verticalFloors = new List<FloorModel>() { new(new PointModel(90, 100), new PointModel(90, 140)), new(new PointModel(30, 140), new PointModel(30, 100)), new(new PointModel(150, 140), new PointModel(150, 100)) },
                Random = new Random((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds % int.MaxValue),
                Tiles = new GridController(200, 200)
            };
            dungeon.FillGround();
            dungeon.FillWalls();
            dungeon.TechInterior();
            return dungeon;
        }

        public static DungeonController CreateAiDungeon()
        {
            var dungeon = new DungeonController
            {
                Rooms = new List<RoomModel>() { CreateRoomAt(25, 25, 25, 25) },
                horizontalFloors = new List<FloorModel>(),
                verticalFloors = new List<FloorModel>(),
                Random = new Random((int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds % int.MaxValue)),
                Tiles = new GridController(50, 50)
            };
            dungeon.FillGround();
            dungeon.FillWalls();
            dungeon.AiRoomInterior();
            return dungeon;
        }

        private static RoomModel CreateRandRoomAtRandomPosition(int xMin, int xMax, int yMin, int yMax, int width, int height, Random random) =>
            new(new PointModel(random.Next(xMin, xMax), random.Next(yMin, yMax)), width, height);

        private static RoomModel CreateRoomAt(int midX, int midY, int height, int width) =>
            new(new PointModel(midX, midY), width, height);

        private void CreateRoomList(int n, int xMin, int xMax, int yMin, int yMax, int width, int height, Random random)
        {
            var alternative = 0;
            while (Rooms.Count < n && alternative < 1000)
            {
                var addRoom = true;
                var newRoom = CreateRandRoomAtRandomPosition(xMin, xMax, yMin, yMax, width, height, random);
                foreach (var room in Rooms)
                    if (newRoom.Overlaps(room))
                    {
                        addRoom = false;
                        break;
                    }
                if (addRoom) Rooms.Add(newRoom);
                alternative++;
            }
        }

        private void CreateMesh(int addPercent)
        {
            var pointList = Rooms.Select(room => room.Middle).ToList();

            var bw = new GraphModel(pointList, new List<EdgeModel>());
            var triList = bw.BowyerWatson();
            var m = GraphModel.GetListToGraph(triList);
            mesh = m.ExtendedSpanningTree(addPercent, Random);
        }

        private void CreateFloors()
        {
            foreach (var edge in mesh.edges)
            {
                RoomModel r1 = null;
                RoomModel r2 = null;
                foreach (var room in Rooms)
                {
                    if (room.Middle.Equals(edge.p)) r1 = room;
                    if (room.Middle.Equals(edge.q)) r2 = room;
                }
                if (r1 != null && r2 != null) r1.FloorConnect(r2, horizontalFloors, verticalFloors);
            }
        }

        private void FillRooms()
        {
            foreach (var room in Rooms)
                for (var a = 0; a < room.Height; a++)
                    for (var b = 0; b < room.Width; b++)
                        Tiles.SetCell((int)(room.TopLeftCorner.X + b), (int)(room.TopLeftCorner.Y + a), CellTypeEnum.GroundCell);
        }

        private void FillFloors()
        {
            foreach (var vFloor in verticalFloors)
            {
                var p = vFloor.start;
                if (vFloor.start.Y > vFloor.end.Y) p = vFloor.end;

                for (var a = -1; a <= 1; a++)
                {
                    const int i2 = 2;
                    for (var b = -1; b < Math.Abs(vFloor.start.Y - vFloor.end.Y) + i2; b++)
                        Tiles.SetCell((int)(p.X - a), (int)(b + p.Y), CellTypeEnum.GroundCell);
                }
            }

            foreach (var floor in horizontalFloors)
            {
                var p = floor.start;
                if (floor.start.X > floor.end.X) p = floor.end;

                for (var i = -1; i <= 1; i++)
                    for (var j = -1; j < Math.Abs(floor.start.X - floor.end.X) + 2; j++)
                        Tiles.SetCell((int)(j + p.X), (int)(p.Y - i), CellTypeEnum.GroundCell);
            }
        }

        private void FillGround()
        {
            FillFloors();
            FillRooms();
        }

        private void FillWalls()
        {
            for (var y = 1; y < Tiles.Height - 1; y++)
                for (var x = 1; x < Tiles.Width - 1; x++)
                {
                    if (Tiles.GetCellType(x, y) != CellTypeEnum.EmptyCell) continue;
                    for (var a = -1; a < 2; a++)
                        for (var b = -1; b < 2; b++)
                            if (Tiles.GetCellType(x + b, y + a) == CellTypeEnum.GroundCell)
                                Tiles.SetCell(x, y, CellTypeEnum.WallCell);
                }
        }

        private void RoomInterior(RoomModel room, GridController roomGrid)
        {
            for (var y = 0; y < roomGrid.Height; y++)
                for (var x = 0; x < roomGrid.Width; x++)
                    if (roomGrid.GetCellType(x, y) != CellTypeEnum.EmptyCell)
                        Tiles.SetCell((int)(room.TopLeftCorner.X + x), (int)(room.TopLeftCorner.Y + y), roomGrid.GetCellType(x, y));
        }

        private void Interior()
        {
            var roomTypes = new List<RoomTypeEnum>() { RoomTypeEnum.PillarRoom, RoomTypeEnum.EmptyRoom, RoomTypeEnum.EmptyRoom, RoomTypeEnum.PillarRoom, RoomTypeEnum.LabyrinthRoom, RoomTypeEnum.LayerRoom };

            RoomInterior(Rooms[0], new RoomTemplateEnum(RoomTypeEnum.SpawnRoom).roomGrid);
            RoomInterior(Rooms[1], new RoomTemplateEnum(RoomTypeEnum.BossRoom).roomGrid);

            for (var i = 2; i < Rooms.Count; i++)
                RoomInterior(Rooms[i], new RoomTemplateEnum(roomTypes[i % roomTypes.Count]).roomGrid);
        }

        private void TechInterior()
        {
            RoomInterior(Rooms[2], new RoomTemplateEnum(RoomTypeEnum.TechDemoBig).roomGrid);
            RoomInterior(Rooms[3], new RoomTemplateEnum(RoomTypeEnum.TechDemoRoom).roomGrid);
        }

        private void AiRoomInterior()
        {
            RoomInterior(Rooms[0], new RoomTemplateEnum(RoomTypeEnum.AiRoom).roomGrid);
        }
    }
}