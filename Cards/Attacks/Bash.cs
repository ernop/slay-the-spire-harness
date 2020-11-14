using System;
using System.Collections.Generic;

namespace StS
{
    public class Bash : Card
    {
        public override string Name => "Bash";
        public override CharacterType CharacterType => CharacterType.IronClad;
        public override CardType CardType => CardType.Attack;
        public override ActionTarget ActionTarget => ActionTarget.Enemy;
        internal override EffectSet Apply(Player player, Enemy enemy, List<Enemy> enemyList, int upgradeCount)
        {
            int amt;
            StatusInstance si;
            if (upgradeCount == 0)
            {
                amt = 8;
                si = new StatusInstance(new Vulnerable(), 2, int.MaxValue);
            }
            else
            {
                amt = 12;
                si = new StatusInstance(new Vulnerable(), 3, int.MaxValue);
            }


            var ef = new EffectSet();
            ef.EnemyReceivesDamage.Add((_) => amt);
            ef.EnemyStatus.Add(si);

            return ef;
        }
    }
}
