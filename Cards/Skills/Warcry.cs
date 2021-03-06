﻿using System.Collections.Generic;

using static StS.Helpers;

namespace StS
{
    public class Warcry : IroncladSkillCard
    {
        public override string Name => nameof(Warcry);

        public override TargetType TargetType => TargetType.Player;
        public override bool RandomEffects => true;
        public override EnergyCostInt CiCanCallEnergyCost(int upgradeCount) => new EnergyCostInt(0);

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, long? key = null)
        {
            var ct = upgradeCount == 0 ? 1 : 2;

            ef.DeckEffect.Add((Deck d, List<string> history) =>
            {
                var drewDesc = "";
                var toDrawPileDesc = "";
                var would = d.WouldDraw(ct);
                drewDesc = SJ(input: would);
                if (HasNoDrawStatus(player))
                {
                    history.Add("Nodraw status so no draw.");
                    return;
                }
                d.ForceDrawCards(player: player, would, ef, history);

                //target is only the card we are going to throw away.
                CardInstance cardToPutOnTopOfDiscardPile;
                if (targets == null)
                {
                    cardToPutOnTopOfDiscardPile = d.GetHand[Rnd.Next(d.GetHand.Count)];

                }
                else
                {
                    if (targets.Count != 1) throw new System.Exception();
                    cardToPutOnTopOfDiscardPile = targets[0];
                }
                toDrawPileDesc = cardToPutOnTopOfDiscardPile.ToString();
                d.AddToDrawPile(cardToPutOnTopOfDiscardPile);
                d.GetHand.Remove(cardToPutOnTopOfDiscardPile);

                history.Add($"warcry drew '{drewDesc}' and put '{toDrawPileDesc}' on top of draw pile");
            });
        }
    }
}
