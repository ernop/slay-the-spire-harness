﻿using System.Collections.Generic;

using static StS.Helpers;

namespace StS
{
    public class Trip : SkillCard
    {
        public override string Name => nameof(Trip);

        public override CardDomain CardDomain => CardDomain.Colorless;

        public override TargetType TargetType => TargetType.Enemy;

        public override int CiCanCallEnergyCost(int upgradeCount) => 0;

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
        {
            ef.EnemyEffect.Status.AddRange(GetStatuses(new Vulnerable(), 2));
        }
    }
}
