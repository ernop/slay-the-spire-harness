using System.Collections.Generic;
using static StS.Helpers;

namespace StS
{
    public class BattleTrance : IroncladSkillCard
    {
        public override string Name => nameof(BattleTrance);
        public override TargetType TargetType => TargetType.Player;
        public override bool RandomEffects => true;
        public override int CiCanCallEnergyCost(int upgradeCount) => 0;
        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, long? key = null)
        {
            var count = upgradeCount == 0 ? 3 : 4;
            ef.DeckEffect.Add((Deck d, List<string> h) =>
            {
                var drawn = d.DrawToHand(targets, count, true, player, ef, h);
                h.Add($"BT Drew: {string.Join(',', drawn)}");
            });
            ef.PlayerEffect.Status.Add(GS(new NoDrawStatus(), 1));
        }
    }
}
