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
        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
        {
            ef.PlayerEffect.InitialBlock = upgradeCount == 0 ? 8 : 11;
            ef.DeckEffect.Add((Deck d) =>
            {
                var drawn = d.DrawToHand(targets, 1, true, ef);
                return $"ShrugDrew: {string.Join(',', drawn)}";
            });
        }
    }
}
