using MysteryWorld.Models.Enums;

namespace MysteryWorld.Models.Interfaces;

public interface IActionType
{
    class Basic : IActionType
    {
        public readonly BasicActionType BasicAction;
        public Basic(BasicActionType basicAction)
        {
            BasicAction = basicAction;
        }
    }

    class Summon : IActionType
    {
        public readonly SummonType SummonType;

        public Summon(SummonType summonType)
        {
            SummonType = summonType;
        }
    }

    class Select : IActionType
    {
        public readonly int SelectedIndex;

        public Select(int selectedIndex)
        {
            SelectedIndex = selectedIndex;
        }
    }
}
