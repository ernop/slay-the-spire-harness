using System.Collections.Generic;

namespace StS
{

    public class Apotheosis : SkillCard
    {
        public override string Name => nameof(Apotheosis);

        public override CardDomain CardDomain => CardDomain.Colorless;
        internal override bool Exhausts(int upgradeCount) => true;
        public override TargetType TargetType => TargetType.Player;

        public override int CiCanCallEnergyCost(int upgradeCount) => upgradeCount == 0 ? 2 : 1;

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, int? key = null)
        {
            ef.DeckEffect.Add((Deck d, List<string> h) =>
            {
                foreach (var sl in new List<IList<CardInstance>>() { d.GetDrawPile, d.GetHand, d.GetDiscardPile, d.GetExhaustPile })
                {
                    foreach (var c in d.GetDrawPile)
                    {
                        c.UpgradeCount++;
                    }
                }
                return "Apotheosis Upgraded all cards";
            });
        }
    }
}
