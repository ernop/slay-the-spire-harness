﻿using System.Collections.Generic;

namespace StS
{
    public class Inflame : PowerCard
    {
        public override string Name => nameof(Inflame);

        public override CharacterType CharacterType => CharacterType.IronClad;

        public override CardType CardType => CardType.Power;
        public override TargetType TargetType => TargetType.Player;
        internal override bool Ethereal(int upgradeCount) => false;
        internal override bool Exhausts(int upgradeCount) => false;
        public override int CiCanCallEnergyCost(int upgradeCount) => 1;

        internal override void Play(EffectSet ef, IEntity source, IEntity target, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
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

            ef.TargetEffect.Status.Add(new StatusInstance(new Strength(), amt));
        }
    }
}
