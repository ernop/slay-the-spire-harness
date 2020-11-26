using System;
using System.Collections.Generic;

namespace StS
{
    public class ShrugItOff : IroncladSkillCard
    {
        public override string Name => nameof(ShrugItOff);

        public override TargetType TargetType => TargetType.Player;

        public override int CiCanCallEnergyCost(int upgradeCount) => 1;

        /// <summary>
        /// for the AI to control, just specify targets.
        /// </summary>
        internal override void Play(EffectSet ef, Entity source, Entity target, int upgradeCount, List<CardInstance> targetCards = null, Deck deck = null)
        {
            ef.TargetEffect.InitialBlock = upgradeCount == 0 ? 8 : 11;
            ef.DeckEffect.Add((Deck d) =>
            {
                d.DrawToHand(this, targetCards, 1, true);
            });
        }
    }
}
