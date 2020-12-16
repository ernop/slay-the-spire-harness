﻿using System.Collections.Generic;

namespace StS
{
    public class TwinStrike : IroncladAttackCard
    {
        public override string Name => nameof(TwinStrike);

        public override int CiCanCallEnergyCost(int upgradeCount) => 1;

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
        {
            var dmg = upgradeCount == 0 ? 5 : 7;
            ef.EnemyEffect.InitialDamage = new List<int>() { dmg, dmg };
        }
    }
}
