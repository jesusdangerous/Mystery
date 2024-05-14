using System.Collections.Generic;
using System.IO;

namespace MysteryWorld.Models
{
    internal class Level
    {
        public int Number { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public List<MapObjectType> Cells { get; set; } = new List<MapObjectType>();

        public Player Sokoban { get; set; } = new Player();
        public List<Box> Boxes { get; set; } = new List<Box>();

        public void Load(int number)
        {
            Number = number;

            var fileName = string.Format("Assets/Levels/level_{0:D2}.txt", Number);

            var lines = File.ReadAllLines(fileName);
            var playerIndex = 0;

            Cells.Clear();
            Boxes.Clear();

            Width = 0;
            Height = 0;
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;

                if (Width == 0)
                {
                    Width = line.Length;
                }

                foreach (var ch in line)
                {
                    switch (ch)
                    {
                        case ' ': Cells.Add(MapObjectType.Empty); break;
                        case 'X': Cells.Add(MapObjectType.Barrier); break;
                        case '.': Cells.Add(MapObjectType.Marker); break;
                        case '*':
                            int boxIndex = Cells.Count;
                            Cells.Add(MapObjectType.Crate);
                            Boxes.Add(new Box(boxIndex % Width, boxIndex / Width));
                            break;
                        case '@':
                            playerIndex = Cells.Count;
                            Cells.Add(MapObjectType.Avatar);
                            break;
                    }
                }
                Height++;
            }

            Sokoban.X = playerIndex % Width;
            Sokoban.Y = playerIndex / Width;
        }

        public MapObjectType Check(int x, int y)
        {
            int index = (y * Width) + x;
            if (index < 0 || index > Cells.Count - 1)
            {
                return MapObjectType.Empty;
            }

            return Cells[index];
        }

        public bool SetPlayer(int dx, int dy)
        {
            var source = Check(Sokoban.X, Sokoban.Y);
            var destination = Check(Sokoban.X + dx, Sokoban.Y + dy);

            if (destination == MapObjectType.Barrier)
                return false;

            if (destination == MapObjectType.Crate || destination == MapObjectType.CrateMarker)
            {
                var after = Check(Sokoban.X + dx * 2, Sokoban.Y + dy * 2);
                if (after == MapObjectType.Barrier)
                    return false;

                if (after == MapObjectType.Crate || after == MapObjectType.CrateMarker)
                    return false;

                if (after == MapObjectType.Marker || after == MapObjectType.Empty)
                {
                    var afterType =
                        after == MapObjectType.Marker ?
                        MapObjectType.CrateMarker :
                        MapObjectType.Crate;
                    SetCell(Sokoban.X + dx * 2, Sokoban.Y + dy * 2, afterType);
                    var found = Boxes.Find((Box box) => {
                        return box.X == Sokoban.X + dx && box.Y == Sokoban.Y + dy;
                    });
                    if (found.X != 0)
                        found.Set(Sokoban.X + dx * 2, Sokoban.Y + dy * 2);

                    var targetType =
                        destination == MapObjectType.CrateMarker ?
                        MapObjectType.AvatarMarker :
                        MapObjectType.Avatar;
                    SetCell(Sokoban.X + dx, Sokoban.Y + dy, targetType);
                }
            }
            else
            {
                var targetType =
                    destination == MapObjectType.Marker ?
                    MapObjectType.AvatarMarker :
                    MapObjectType.Avatar;
                SetCell(Sokoban.X + dx, Sokoban.Y + dy, targetType);
            }

            var sourceType =
                source == MapObjectType.Avatar ?
                MapObjectType.Empty :
                MapObjectType.Marker;
            SetCell(Sokoban.X, Sokoban.Y, sourceType);

            Sokoban.SetPosition(Sokoban.X + dx, Sokoban.Y + dy);

            return true;
        }

        public void SetCell(int x, int y, MapObjectType type)
        {
            var index = (y * Width) + x;
            Cells[index] = type;
        }

        public bool CheckLevelComplete()
        {
            foreach (var cell in Cells)
                if (cell == MapObjectType.Marker)
                    return false;

            return true;
        }
    }
}
