using System.Collections.Generic;

namespace StS
{
    public static class AllPotions
    {
        public static Dictionary<string, Potion> Potions { get; private set; } = GetAllPotions();

        public static Dictionary<string, Potion> GetAllPotions()
        {
            var potions = new Dictionary<string, Potion>();
            potions[nameof(StrengthPotion)] = new StrengthPotion();
            potions[nameof(EssenceOfSteel)] = new EssenceOfSteel();
            return potions;
        }
    }
}
