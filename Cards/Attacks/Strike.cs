using System.Collections.Generic;

namespace StS
{
    public class Strike : Card
    {
        public override string Name => nameof(Strike);

        public override CharacterType CharacterType => CharacterType.IronClad;


        public override CardType CardType => CardType.Attack;

        public override ActionTarget ActionTarget => ActionTarget.Enemy;

        internal override EffectSet Apply(Player player, Enemy enemy, List<Enemy> enemyList, int upgradeCount)
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

            var ef = new EffectSet();
            ef.EnemyReceivesDamage.Add((_) => amount);
            return ef;
        }
    }
}
