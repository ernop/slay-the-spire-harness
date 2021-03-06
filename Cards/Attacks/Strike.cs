﻿using System.Collections.Generic;

namespace StS
{
    public class Strike : IroncladAttackCard
    {
        public override string Name => nameof(Strike);
        public override EnergyCostInt CiCanCallEnergyCost(int upgradeCount) => new EnergyCostInt(1);

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, long? key = null)
        {
            int dmg = upgradeCount == 0 ? 6 : 9;
            ef.EnemyEffect.SetInitialDamage(dmg);
        }
    }
}
