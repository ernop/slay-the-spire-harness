﻿using System.Collections.Generic;

namespace StS
{
    public class Entrench : IroncladSkillCard
    {
        public override string Name => nameof(Entrench);

        public override TargetType TargetType => TargetType.Player;

        public override EnergyCostInt CiCanCallEnergyCost(int upgradeCount) => upgradeCount == 0 ? new EnergyCostInt(2) : new EnergyCostInt(1);

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, long? key = null)
        {
            //is it as simple as this?
            ef.PlayerEffect.AddBlockStep("Entrench", player.Block, 10,true);
        }
    }
}
