﻿using System.Collections.Generic;

namespace StS
{
    public class IronWave : IroncladAttackCard
    {
        public override string Name => nameof(IronWave);
        public override int CiCanCallEnergyCost(int upgradeCount) => 1;
        internal override void Play(EffectSet ef, IEntity source, IEntity target, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
        {
            int dmg;
            int block;
            if (upgradeCount == 0)
            {
                dmg = 5;
                block = 5;
            }
            else
            {
                dmg = 7;
                block = 7;
            }

            ef.TargetEffect.InitialDamage = new List<int>() { dmg };
            ef.SourceEffect.InitialBlock = block;
        }
    }
}
