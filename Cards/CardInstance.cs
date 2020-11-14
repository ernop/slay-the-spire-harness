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

        public EffectSet Apply(Player player, Enemy enemy, List<Enemy> enemyList, int upgradeCount)
        {
            if (Helpers.PrintDetails)
            {
                Console.WriteLine($"\tplaying card {this}");
            }
            var ef = Card.Apply(player, enemy, enemyList, UpgradeCount);
            return ef;
        }

        public override string ToString()
        {
            var upgrade = UpgradeCount > 0 ? "+" : "";
            return $"{Card}{upgrade}";
        }

        internal ActionTarget GetActionTarget()
        {
            //here determine if we are AOE or not;
            return Card.ActionTarget;
        }
    }
}
