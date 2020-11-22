﻿using System.Collections.Generic;
using System.Linq;

namespace StS
{
    public class Clash : IroncladAttackCard
    {
        public override string Name => nameof(Clash);

        public override int CiCanCallEnergyCost(int upgradeCount) => 0;

        public override bool Playable(List<CardInstance> hand) => hand.All(el => el.Card.CardType == CardType.Attack);

        internal override void Play(EffectSet ef, Entity source, Entity target, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
        {
            var dmg = upgradeCount == 0 ? 14 : 18;
            ef.TargetEffect.InitialDamage = new List<int>() { dmg };
        }
    }
}
