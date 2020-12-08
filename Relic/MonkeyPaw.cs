using System;

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
                Func<Deck, string> makeOneCostZero = (Deck) =>
                 {
                     var ci = SelectNonZeroCostCard(Deck.GetHand);
                     if (ci != null)
                     {
                         ci.PerTurnOverrideEnergyCost = 0;
                         return $"monkey paw set {ci} to cost zero.";
                     }
                     else
                     {
                         return $"monkey paw had no card to reduce in value..";
                     }
                 };
                ef.DeckEffect.Add(makeOneCostZero);
            }
        }

        internal override Relic Copy() => new MonkeyPaw();
    }
}
