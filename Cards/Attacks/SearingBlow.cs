﻿using System.Collections.Generic;

namespace StS
{
    public class SearingBlow : IroncladAttackCard
    {

        public override string Name => nameof(SearingBlow);
        public override EnergyCostInt CiCanCallEnergyCost(int upgradeCount) => new EnergyCostInt(2);
        public override bool MultiUpgrade => true;
        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, long? key = null)
        {
            var amt = 4;
            var tot = 0;
            while (upgradeCount > 0)
            {
                tot += amt;
                amt++;
                upgradeCount--;
            }
            var dmg = 12 + tot;
            ef.EnemyEffect.SetInitialDamage(dmg);
        }
    }
}
