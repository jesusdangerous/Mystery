using MysteryWorld.Models.Enums;

namespace MysteryWorld.Models
{
    internal static class ElementalModel
    {
        private const float Inf = 0.7f;
        private const float Sup = 2.1f;

        public static float ElementalEffectiveness(this ElementType atkElement, ElementType defElement)
        {
            if (atkElement == defElement || atkElement == ElementType.Neutral || defElement == ElementType.Neutral)
                return 1;

            return atkElement switch
            {
                ElementType.Fire => defElement is ElementType.Lightning or ElementType.Magic ? Sup : Inf,
                ElementType.Ghost => defElement is ElementType.Fire ? Sup : Inf,
                ElementType.Lightning => defElement is ElementType.Ghost ? Sup : Inf,
                ElementType.Magic => defElement is ElementType.Lightning or ElementType.Ghost ? Sup : Inf,
                _ => 1
            };
        }

        public static bool ElementIsEffective(this ElementType atkElement, ElementType defElement)
        {
            if (atkElement == defElement || atkElement == ElementType.Neutral || defElement == ElementType.Neutral)
                return false;

            return atkElement switch
            {
                ElementType.Fire => defElement is ElementType.Lightning or ElementType.Magic,
                ElementType.Ghost => defElement is ElementType.Fire,
                ElementType.Lightning => defElement is ElementType.Ghost,
                ElementType.Magic => defElement is ElementType.Lightning or ElementType.Ghost,
                _ => false
            };
        }
    }
}
