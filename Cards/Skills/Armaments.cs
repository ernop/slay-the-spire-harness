using System.Collections.Generic;

namespace StS
{
    public class Armaments : IroncladSkillCard
    {
        public override string Name => nameof(Armaments);

        public override TargetType TargetType => TargetType.Player;

        public override EnergyCostInt CiCanCallEnergyCost(int upgradeCount) => new EnergyCostInt(1);

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, long? key = null)
        {
            ef.DeckEffect.Add((Deck d, List<string> h) =>
            {
                ef.PlayerEffect.AddBlockStep("Defend", 5);
                if (upgradeCount == 0)
                {
                    var ci = deck.ChooseCardFromHand((ci) => ci.Upgradeable(), "Upgrading with Armaments");
                    if (ci == null)
                    {
                        h.Add($"No upgradeable card in hand");
                        return;
                    }
                    ci.Upgrade();
                    h.Add($"Armaments Upgraded {ci} in hand");
                }
                if (upgradeCount == 1)
                {
                    var upgradeCount = 0;
                    foreach (var ci in d.GetHand)
                    {
                        var bef = ci.UpgradeCount;
                        ci.Upgrade();
                        if (ci.UpgradeCount != bef)
                        {
                            upgradeCount++;
                        }

                    }
                    h.Add($"Armaments Upgraded {upgradeCount} cards in hand");
                }
            });
        }
    }
}
