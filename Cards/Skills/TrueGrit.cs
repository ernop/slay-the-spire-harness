using System.Collections.Generic;

namespace StS
{
    public class TrueGrit : IroncladSkillCard
    {
        public override string Name => nameof(TrueGrit);

        public override TargetType TargetType => TargetType.Player;

        public override EnergyCostInt CiCanCallEnergyCost(int upgradeCount) => new EnergyCostInt(1);

        /// <summary>
        /// Bit annoying that targets actually covers two cases:
        /// 1. AI forcing a certain random choice
        /// 2. Normal play of the upgraded card, specifying the target
        /// </summary>
        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, long? key = null)
        {
            var amt = upgradeCount == 0 ? 7 : 10;
            ef.PlayerEffect.AddBlockStep("TrueGrit", amt);

            ef.DeckEffect.Add((Deck d, List<string> h) =>
            {
                //target determination:
                // upgraded + specify
                // upgraded + ask user
                // upgraded + default to random (!interactiveContext)
                // nonupgraded + specify
                // nonupgraded + random
                CardInstance target;
                var spec = "";
                if (targets == null)
                {
                    if (upgradeCount == 0)
                    {
                        target = d.GetRandomCardFromHand();
                        spec = "Picked random card from hand";
                    }
                    else
                    {
                        target = d.ChooseCardFromHand(filter: null, prompt: "Pick a card to exhaust with True Grit");
                        spec = "Prompted to pick card from hand.";
                    }
                }
                else
                {
                    target = targets[0];
                    spec = "used target";
                }

                if (target == null)
                {
                    h.Add($"True Grit: {spec} Nothing to exhaust");
                }
                else
                {
                    h.Add($"True Grit: {spec}: {target}");
                    d.ExhaustFromHand(target, ef, h);
                }
            });
        }
    }
}
