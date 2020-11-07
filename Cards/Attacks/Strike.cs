using System.Collections.Generic;

namespace StS
{
    public class Strike : Card
    {
        public override string Name => nameof(Strike);

        public override CharacterType CharacterType => CharacterType.IronClad;

        public override CardType CardType => CardType.Attack;

        public override void Apply(Player player, Enemy enemy, List<Enemy> enemyList, int upgradeCount)
        {
            int amount;
            if (upgradeCount == 0)
            {
                amount = 6;
            }
            else
            {
                amount = 9;
            }

            amount = player.AdjustDealtDamage(amount);

            enemy.ApplyDamage(amount);
        }
    }
}
