using System;
using System.Collections.Generic;
using System.Text;

namespace StS
{
    public class Inflame : Card
    {
        public override string Name => nameof(Inflame);

        public override CharacterType CharacterType => CharacterType.IronClad;

        public override CardType CardType => CardType.Power;

        internal override EffectSet Apply(Player player, Enemy enemy, List<Enemy> enemyList, int upgradeCount)
        {
            var amt = 0;
            if (upgradeCount == 0)
            {
                amt = 2;
            }
            else
            {
                amt = 3;
            }

            var ef = new EffectSet();
            ef.PlayerStatus.Add(new StatusInstance(new Strength(), int.MaxValue, amt));

            return ef;
        }
    }
}
