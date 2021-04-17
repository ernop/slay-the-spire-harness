﻿using System.Collections.Generic;

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
                if (targets == null)
                {
                    var target = d.GetRandomCardFromHand();
                    d.ExhaustFromHand(target, ef);
                    if (target == null)
                    {
                        h.Add($"True Grit: Nothing to exhaust");
                    }
                    else
                    {
                        h.Add($"True Grit: Exhausted {target} by random");
                    }
                }
                else
                {
                    if (targets.Count != 1)
                    {
                        throw new System.Exception();
                    }
                    var hand = d.GetHand;
                    var ci = targets[0];
                    if (!hand.Contains(ci))
                    {
                        throw new System.Exception();
                    }
                    d.ExhaustFromHand(ci, ef);
                    h.Add($"True Grit: Exhausted {ci} by by specification.");
                }
            });
        }
    }
}
