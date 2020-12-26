using System;
using System.Collections.Generic;
using static StS.Helpers;

namespace StS
{
    public class MonkeyPaw : Relic
    {
        public override string Name => nameof(MonkeyPaw);

        public override void CardPlayed(Card card, EffectSet ef, IEntity player, IEntity enemy)
        {
            if (card.CardType == CardType.Power)
            {
                ef.DeckEffect.Add((Deck d, List<string> h) =>
                {
                    var ci = SelectNonZeroCostCard(d.GetHand);
                    if (ci != null)
                    {
                        ci.PerTurnOverrideEnergyCost = 0;
                        return $"monkey paw set {ci} to cost zero.";
                    }
                    else
                    {
                        return $"monkey paw had no card to reduce in value..";
                    }
                });
            }
        }

        internal override Relic Copy() => new MonkeyPaw();
    }
}
