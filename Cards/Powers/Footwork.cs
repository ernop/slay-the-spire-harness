﻿using System.Collections.Generic;

namespace StS
{

    public class Footwork : PowerCard
    {
        public override string Name => nameof(Footwork);

        public override CardDomain CardDomain => CardDomain.Silent;
        public override int CiCanCallEnergyCost(int upgradeCount) => 1;
        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null)
        {
            int amt;
            if (upgradeCount == 0)
            {
                amt = 2;
            }
            else
            {
                amt = 3;
            }

            ef.PlayerEffect.Status.Add(new StatusInstance(new Dexterity(), amt));
        }
    }
}
