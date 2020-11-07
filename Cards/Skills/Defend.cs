using System.Collections.Generic;

namespace StS
{
    public class Defend : Card
    {
        public override string Name => nameof(Defend);
        public override CharacterType CharacterType => CharacterType.IronClad;
        public override CardType CardType => CardType.Skill;

        public override void Apply(Player player, Enemy enemy, List<Enemy> enemyList, int upgradeCount)
        {
            int amount;
            if (upgradeCount == 0)
            {
                amount = 5;
            }
            else
            {
                amount = 6;
            }

            foreach (var status in player.Statuses)
            {
                status.GainsBlock(amount);
            }


            player.ApplyBlock(amount);
        }
    }
}
