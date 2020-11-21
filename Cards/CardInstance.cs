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

        public void Play(EffectSet ef, Entity source, Entity target, int upgradeCount)
        {
            if (Helpers.PrintDetails)
            {
                Console.WriteLine($"\tplaying card {this}");
            }
            
            Card.Play(ef, source, target, UpgradeCount);
        }

        public override string ToString()
        {
            var upgrade = UpgradeCount > 0 ? "+" : "";
            return $"{Card}{upgrade}";
        }
    }
}
