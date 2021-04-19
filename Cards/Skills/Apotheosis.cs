using System.Collections.Generic;

namespace StS
{
    public class Apotheosis : SkillCard
    {
        public override string Name => nameof(Apotheosis);

        public override CardDomain CardDomain => CardDomain.Colorless;
        internal override bool Exhausts(int upgradeCount) => true;
        public override TargetType TargetType => TargetType.Player;

        public override EnergyCostInt CiCanCallEnergyCost(int upgradeCount) => upgradeCount == 0 ? new EnergyCostInt(2) : new EnergyCostInt(1);
        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, long? key = null)
        {
            ef.DeckEffect.Add((Deck d, List<string> h) =>
            {
                var upgradeCount = 0;
                foreach (var sl in new List<IList<CardInstance>>() { d.GetDrawPile, d.GetHand, d.GetDiscardPile, d.GetExhaustPile })
                {
                    foreach (var c in sl)
                    {
                        var bef = c.UpgradeCount;
                        c.Upgrade();
                        if (c.UpgradeCount != bef)
                        {
                            upgradeCount++;
                        }
                    }
                }
                h.Add($"Apotheosis Upgraded all cards {upgradeCount}");
            });
        }
    }
}
