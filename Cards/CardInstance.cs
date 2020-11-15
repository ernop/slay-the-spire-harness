using System;
using System.Collections.Generic;

namespace StS
{
    public class CardInstance
    {
        public int UpgradeCount { get; set; }
        public Card Card { get; set; }
        
        public CardInstance(Card card, int upgradeCount)
        {
            Card = card;
            UpgradeCount = upgradeCount;
        }

        public EffectSet Apply(Entity source, Entity target, int upgradeCount)
        {
            if (Helpers.PrintDetails)
            {
                Console.WriteLine($"\t playing card {this}");
            }
            var ef = Card.Apply(source, target, UpgradeCount);
            return ef;
        }

        public override string ToString()
        {
            var upgrade = UpgradeCount > 0 ? "+" : "";
            return $"{Card}{upgrade}";
        }
    }
}
