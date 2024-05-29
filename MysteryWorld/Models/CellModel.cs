using MysteryWorld.Models.Enums;

namespace MysteryWorld.Models
{
    public sealed class CellModel
    {
        public CellTypeEnum CellType;

        public CellModel(CellTypeEnum cellType)
        {
            CellType = cellType;
        }
    }
}
