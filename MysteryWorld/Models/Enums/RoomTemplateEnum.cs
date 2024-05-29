using System;
using MysteryWorld.Controllers;

namespace MysteryWorld.Models.Enums
{
    internal sealed class RoomTemplateEnum
    {
        public readonly GridController roomGrid;

        public RoomTemplateEnum(RoomTypeEnum roomType)
        {
            roomGrid = new GridController(19, 15);
            switch (roomType)
            {
                case RoomTypeEnum.EmptyRoom:
                    EmptyRoomInterior();
                    break;
                case RoomTypeEnum.LabyrinthRoom:
                    LabyrinthRoomInterior();
                    break;
                case RoomTypeEnum.LayerRoom:
                    LayerRoomInterior();
                    break;
                case RoomTypeEnum.PillarRoom:
                    PillarRoomInterior();
                    break;
                case RoomTypeEnum.SpawnRoom:
                    SpawnRoomInterior();
                    break;
                case RoomTypeEnum.BossRoom:
                    BossRoomInterior();
                    break;
                case RoomTypeEnum.TechDemoRoom:
                    roomGrid = new GridController(45, 25);
                    TechDemoRoomInterior();
                    break;
                case RoomTypeEnum.TechDemoBig:
                    roomGrid = new GridController(45, 45);
                    TechBigInterior();
                    break;
                case RoomTypeEnum.AiRoom:
                    roomGrid = new GridController(25, 25);
                    AiRoomInterior();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(roomType), roomType, null);
            }
        }

        private void SpawnRoomInterior()
        {
            roomGrid.SetCell(8, 6, CellTypeEnum.PillarTopCell);
            roomGrid.SetCell(9, 6, CellTypeEnum.ShrineTopCell);
            roomGrid.SetCell(10, 6, CellTypeEnum.PillarTopCell);

            roomGrid.SetCell(8, 7, CellTypeEnum.PillarMidCell);
            roomGrid.SetCell(9, 7, CellTypeEnum.ShrineMidCell);
            roomGrid.SetCell(10, 7, CellTypeEnum.PillarMidCell);

            roomGrid.SetCell(8, 8, CellTypeEnum.PillarBotCell);
            roomGrid.SetCell(9, 8, CellTypeEnum.ShrineBotCell);
            roomGrid.SetCell(10, 8, CellTypeEnum.PillarBotCell);
        }

        private void BossRoomInterior()
        {
        }

        private void EmptyRoomInterior()
        {
        }

        private void LabyrinthRoomInterior()
        {
            for (var i = 0; i < 2; i++)
            {
                roomGrid.SetCell(i, 9, CellTypeEnum.DestructAbleWallCell);
                roomGrid.SetCell(18 - i, 5, CellTypeEnum.DestructAbleWallCell);
                roomGrid.SetCell(7, i, CellTypeEnum.DestructAbleWallCell);
                roomGrid.SetCell(11, 14 - i, CellTypeEnum.DestructAbleWallCell);
                roomGrid.SetCell(8, 5 + i, CellTypeEnum.DestructAbleWallCell);
                roomGrid.SetCell(10, 9 - i, CellTypeEnum.DestructAbleWallCell);
            }

            for (var i = 0; i < 4; i++)
            {
                roomGrid.SetCell(9 + i, 5, CellTypeEnum.DestructAbleWallCell);
                roomGrid.SetCell(6 + i, 9, CellTypeEnum.DestructAbleWallCell);
            }

            for (var i = 0; i < 8; i++)
            {
                roomGrid.SetCell(2, 2 + i, CellTypeEnum.DestructAbleWallCell);
                roomGrid.SetCell(16, 5 + i, CellTypeEnum.DestructAbleWallCell);
            }

            for (var i = 0; i < 10; i++)
            {
                roomGrid.SetCell(5, i, CellTypeEnum.DestructAbleWallCell);
                roomGrid.SetCell(13, 14 - i, CellTypeEnum.DestructAbleWallCell);
                roomGrid.SetCell(7 + i, 2, CellTypeEnum.DestructAbleWallCell);
                roomGrid.SetCell(2 + i, 12, CellTypeEnum.DestructAbleWallCell);
            }
        }

        private void LayerRoomInterior()
        {
            for (var i = 0; i < 11; i++)
            {
                roomGrid.SetCell(4 + i, 2, CellTypeEnum.DestructAbleWallCell);
                roomGrid.SetCell(4 + i, 12, CellTypeEnum.DestructAbleWallCell);
            }
            for (var i = 0; i < 3; i++)
            {
                roomGrid.SetCell(8 + i, 5, CellTypeEnum.DestructAbleWallCell);
                roomGrid.SetCell(8 + i, 9, CellTypeEnum.DestructAbleWallCell);
            }
            for (var i = 0; i < 5; i++)
            {
                roomGrid.SetCell(2, 5 + i, CellTypeEnum.DestructAbleWallCell);
                roomGrid.SetCell(5, 5 + i, CellTypeEnum.DestructAbleWallCell);
                roomGrid.SetCell(13, 5 + i, CellTypeEnum.DestructAbleWallCell);
                roomGrid.SetCell(16, 5 + i, CellTypeEnum.DestructAbleWallCell);
            }
        }

        private void PillarRoomInterior()
        {
            for (var y = -1; y <= 1; y++)
                for (var x = -1; x <= 1; x++)
                {
                    roomGrid.SetCell(3 + x, 3 + y, CellTypeEnum.DestructAbleWallCell);
                    roomGrid.SetCell(15 + x, 11 + y, CellTypeEnum.DestructAbleWallCell);
                    roomGrid.SetCell(9 + x, 7 + y, CellTypeEnum.DestructAbleWallCell);
                    roomGrid.SetCell(3 + x, 11 + y, CellTypeEnum.DestructAbleWallCell);
                    roomGrid.SetCell(15 + x, 3 + y, CellTypeEnum.DestructAbleWallCell);
                }
        }

        private void TechBigInterior()
        {
            roomGrid.SetCell(0, 25, CellTypeEnum.PillarTopCell);
            roomGrid.SetCell(1, 25, CellTypeEnum.ShrineTopCell);
            roomGrid.SetCell(2, 25, CellTypeEnum.PillarTopCell);

            roomGrid.SetCell(0, 26, CellTypeEnum.PillarMidCell);
            roomGrid.SetCell(1, 26, CellTypeEnum.ShrineMidCell);
            roomGrid.SetCell(2, 26, CellTypeEnum.PillarMidCell);

            roomGrid.SetCell(0, 27, CellTypeEnum.PillarBotCell);
            roomGrid.SetCell(1, 27, CellTypeEnum.ShrineBotCell);
            roomGrid.SetCell(2, 27, CellTypeEnum.PillarBotCell);
        }

        private void TechDemoRoomInterior()
        {
            TechDividers();
            TechTopLeft();
            TechTopRight();
            TechBotLeft();
            TechBotRight();
        }

        private void TechDividers()
        {
            for (var i = 0; i <= 16; i++)
            {
                roomGrid.SetCell(i, 10, CellTypeEnum.WallCell);
                roomGrid.SetCell(44 - i, 10, CellTypeEnum.WallCell);
            }

            for (var i = 0; i <= 20; i++)
            {
                roomGrid.SetCell(i, 14, CellTypeEnum.WallCell);
                roomGrid.SetCell(44 - i, 14, CellTypeEnum.WallCell);
            }

            for (var i = 0; i <= 10; i++)
            {
                roomGrid.SetCell(20, i, CellTypeEnum.WallCell);
                roomGrid.SetCell(24, i, CellTypeEnum.WallCell);
            }

            for (var i = 0; i < 4; i++)
            {
                roomGrid.SetCell(20, 18 + i, CellTypeEnum.WallCell);
                roomGrid.SetCell(24, 18 + i, CellTypeEnum.WallCell);
            }
            roomGrid.SetCell(20, 15, CellTypeEnum.WallCell);
            roomGrid.SetCell(20, 24, CellTypeEnum.WallCell);
            roomGrid.SetCell(24, 15, CellTypeEnum.WallCell);
            roomGrid.SetCell(24, 24, CellTypeEnum.WallCell);

        }

        private void TechTopLeft()
        {
            roomGrid.SetCell(4, 5, CellTypeEnum.WallCell);
            for (var i = 0; i <= 10; i++)
                roomGrid.SetCell(3 + i, 7, CellTypeEnum.WallCell);
            for (var i = 0; i <= 9; i++)
                roomGrid.SetCell(4 + i, 4, CellTypeEnum.WallCell);
            for (var i = 0; i <= 4; i++)
                roomGrid.SetCell(2, 3 + i, CellTypeEnum.WallCell);
            for (var i = 0; i <= 2; i++)
                roomGrid.SetCell(13, 5 + i, CellTypeEnum.WallCell);
            for (var i = 0; i <= 7; i++)
                roomGrid.SetCell(16, 2 + i, CellTypeEnum.WallCell);
            for (var i = 0; i <= 13; i++)
                roomGrid.SetCell(2 + i, 2, CellTypeEnum.WallCell);
        }

        private void TechTopRight()
        {
            for (var y = 0; y <= 3; y++)
                for (var x = 0; x <= 3; x++)
                    roomGrid.SetCell(33 + x, 3 + y, CellTypeEnum.WallCell);

            for (var i = 0; i < 6; i++)
            {
                roomGrid.SetCell(27, 2 + i, CellTypeEnum.WallCell);
                roomGrid.SetCell(42, 2 + i, CellTypeEnum.WallCell);
            }

            for (var i = 0; i < 2; i++)
            {
                roomGrid.SetCell(30, 1 + i, CellTypeEnum.WallCell);
                roomGrid.SetCell(30, 4 + i, CellTypeEnum.WallCell);
                roomGrid.SetCell(30, 7 + i, CellTypeEnum.WallCell);
                roomGrid.SetCell(39, 1 + i, CellTypeEnum.WallCell);
                roomGrid.SetCell(39, 4 + i, CellTypeEnum.WallCell);
                roomGrid.SetCell(39, 7 + i, CellTypeEnum.WallCell);
            }
        }

        private void TechBotLeft()
        {
            for (var i = 0; i < 5; i++)
            {
                roomGrid.SetCell(2 + i, 16, CellTypeEnum.WallCell);
                roomGrid.SetCell(2 + i, 23, CellTypeEnum.WallCell);
                roomGrid.SetCell(4 + i, 18, CellTypeEnum.WallCell);
                roomGrid.SetCell(4 + i, 21, CellTypeEnum.WallCell);
            }

            for (var i = 0; i < 6; i++)
                roomGrid.SetCell(2, 17 + i, CellTypeEnum.WallCell);
            for (var i = 0; i < 4; i++)
                roomGrid.SetCell(9, 18 + i, CellTypeEnum.WallCell);

            for (var i = 0; i < 8; i++)
            {
                roomGrid.SetCell(12 + i, 18, CellTypeEnum.WallCell);
                roomGrid.SetCell(12 + i, 21, CellTypeEnum.WallCell);
            }

            for (var i = 0; i < 2; i++)
            {
                roomGrid.SetCell(12, 15 + i, CellTypeEnum.WallCell);
                roomGrid.SetCell(14, 16 + i, CellTypeEnum.WallCell);
                roomGrid.SetCell(16, 15 + i, CellTypeEnum.WallCell);
                roomGrid.SetCell(18, 16 + i, CellTypeEnum.WallCell);
                roomGrid.SetCell(12, 22 + i, CellTypeEnum.WallCell);
            }
        }

        private void TechBotRight()
        {
            for (var i = 0; i < 6; i++)
            {
                roomGrid.SetCell(29, 16 + i, CellTypeEnum.DestructAbleWallCell);
                roomGrid.SetCell(28 + i, 22, CellTypeEnum.DestructAbleWallCell);
                roomGrid.SetCell(39 + i, 16, CellTypeEnum.DestructAbleWallCell);
                roomGrid.SetCell(39 + i, 18, CellTypeEnum.DestructAbleWallCell);
                roomGrid.SetCell(39 + i, 21, CellTypeEnum.DestructAbleWallCell);
                roomGrid.SetCell(39 + i, 23, CellTypeEnum.DestructAbleWallCell);
            }

            for (var i = 0; i < 8; i++)
            {
                roomGrid.SetCell(27, 15 + i, CellTypeEnum.DestructAbleWallCell);
                roomGrid.SetCell(36, 17 + i, CellTypeEnum.DestructAbleWallCell);
            }

            for (var i = 0; i < 3; i++)
            {
                roomGrid.SetCell(34, 15 + i, CellTypeEnum.DestructAbleWallCell);
                roomGrid.SetCell(31 + i, 18, CellTypeEnum.DestructAbleWallCell);
            }

            for (var i = 0; i < 2; i++)
            {
                roomGrid.SetCell(33, 20 + i, CellTypeEnum.DestructAbleWallCell);
                roomGrid.SetCell(30 + i, 16, CellTypeEnum.DestructAbleWallCell);
            }

            roomGrid.SetCell(34, 18, CellTypeEnum.DestructAbleWallCell);
            roomGrid.SetCell(39, 19, CellTypeEnum.DestructAbleWallCell);
            roomGrid.SetCell(43, 19, CellTypeEnum.DestructAbleWallCell);
            roomGrid.SetCell(41, 20, CellTypeEnum.DestructAbleWallCell);
            roomGrid.SetCell(30, 23, CellTypeEnum.DestructAbleWallCell);
            roomGrid.SetCell(28, 24, CellTypeEnum.DestructAbleWallCell);
            roomGrid.SetCell(32, 24, CellTypeEnum.DestructAbleWallCell);
        }

        private void AiRoomInterior()
        {
            roomGrid.SetCell(0, 5, CellTypeEnum.PillarTopCell);
            roomGrid.SetCell(1, 5, CellTypeEnum.ShrineTopCell);
            roomGrid.SetCell(2, 5, CellTypeEnum.PillarTopCell);

            roomGrid.SetCell(0, 6, CellTypeEnum.PillarMidCell);
            roomGrid.SetCell(1, 6, CellTypeEnum.ShrineMidCell);
            roomGrid.SetCell(2, 6, CellTypeEnum.PillarMidCell);

            roomGrid.SetCell(0, 7, CellTypeEnum.PillarBotCell);
            roomGrid.SetCell(1, 7, CellTypeEnum.ShrineBotCell);
            roomGrid.SetCell(2, 7, CellTypeEnum.PillarBotCell);
        }
    }
}