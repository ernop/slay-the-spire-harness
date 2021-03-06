﻿using System.Collections.Generic;

namespace StS
{
    public class FeelNoPain : IroncladPowerCard
    {
        public override string Name => nameof(FeelNoPain);

        public override EnergyCostInt CiCanCallEnergyCost(int upgradeCount) => new EnergyCostInt(1);

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, long? key = null)
        {
            var amt = upgradeCount == 0 ? 3 : 4;
            ef.PlayerEffect.Status.Add(new StatusInstance(new FeelNoPainStatus(), amt));
        }
    }
}
