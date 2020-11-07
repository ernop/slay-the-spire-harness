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

        public void Play(Player player, Enemy enemy, List<Enemy> enemyList)
        {
            if (Helpers.PrintDetails)
            {
                Console.WriteLine($"\tplaying card {this}");
            }
            Card.Apply(player, enemy, enemyList, UpgradeCount);
        }

        public override string ToString()
        {
            var upgrade = UpgradeCount > 0 ? "+" : "";
            return $"{Card}{upgrade}";
        }
    }
}
