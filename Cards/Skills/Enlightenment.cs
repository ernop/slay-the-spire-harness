using System.Collections.Generic;

namespace StS
{
    public class Enlightenment : SkillCard
    {
        public override string Name => nameof(Enlightenment);

        public override CardDomain CardDomain => CardDomain.Colorless;

        public override TargetType TargetType => TargetType.Player;

        public override int CiCanCallEnergyCost(int upgradeCount) => 0;

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, long? key = null)
        {
            if (upgradeCount == 0)
            {
                ef.DeckEffect.Add((Deck d, List<string> h) =>
                {
                    var count = 0;
                    foreach (var ci in d.GetHand)
                    {
                        if (ci.EnergyCost() > 1)
                        {
                            ci.PerTurnOverrideEnergyCost = 1;
                            count++;
                        }
                    }
                    h.Add($"Upgraded {count} cards to cost 1 this turn");
                });
            }
            else
            {
                ef.DeckEffect.Add((Deck d, List<string> h) =>
                {
                    var count = 0;
                    foreach (var ci in d.GetHand)
                    {
                        if (ci.EnergyCost() > 1)
                        {
                            ci.PerFightOverrideEnergyCost = 1;
                            count++;
                        }

                    }
                    h.Add($"Upgraded {count} cards to cost 1 this fight");
                });
            }
        }
    }
}
