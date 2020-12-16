﻿using System.Collections.Generic;

namespace StS
{
    public class Anger : IroncladAttackCard
    {
        public override string Name => nameof(Anger);

        public override int CiCanCallEnergyCost(int upgradeCount) => 0;

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
        {
            var dmg = upgradeCount == 0 ? 6 : 8;
            ef.EnemyEffect.InitialDamage = new List<int>() { dmg };
            ef.DeckEffect.Add((Deck d) =>
            {
                var copy = new CardInstance(this, upgradeCount);
                d.PutInDiscardPile(copy);
                return "Duplicated Anger into discard pile";
            });
        }
    }
}
