﻿using System.Collections.Generic;

namespace StS
{
    public class Anger : IroncladAttackCard
    {
        public override string Name => nameof(Anger);

        public override EnergyCostInt CiCanCallEnergyCost(int upgradeCount) => new EnergyCostInt(0);

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, long? key = null)
        {
            var dmg = upgradeCount == 0 ? 6 : 8;
            ef.EnemyEffect.SetInitialDamage(dmg);
            ef.DeckEffect.Add((Deck d, List<string> h) =>
            {
                var copy = new CardInstance(this, upgradeCount);
                d.AddToDiscardPile(copy);
                h.Add("Duplicated Anger into discard pile");
            });
        }
    }
}
