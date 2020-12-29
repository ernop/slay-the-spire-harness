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
        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, long? key = null)
        {
            var amt = upgradeCount == 0 ? 8 : 11;
            ef.PlayerEffect.AddBlockStep("Shrug", amt);
            ef.DeckEffect.Add((Deck d,  List<string> h) =>
            {
                var drawn = d.DrawToHand(targets, 1, true, ef, h);
                return $"ShrugDrew: {string.Join(',', drawn)}";
            });
        }
    }
}
