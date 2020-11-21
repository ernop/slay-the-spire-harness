using System;
using System.Collections.Generic;

using static StS.Helpers;

namespace StS
{
    public class MonkeyPaw : Relic
    {
        public override string Name => nameof(MonkeyPaw);

        public override void CardPlayed(Card card, EffectSet ef, Entity player, Entity enemy)
        {
            if (card.CardType == CardType.Power)
            {
                Action<List<CardInstance>> thingie = (List<CardInstance> cis) =>
                {
                    var ci = SelectNonZeroCostCard(cis);
                    ci.OverrideEnergyCost = 0;
                };
                ef.HandEffect.Add(thingie);
            }
        }
    }
}
