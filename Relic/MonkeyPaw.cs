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
                    if (ci != null)
                    {
                        Console.WriteLine($"monkey paw set {ci} to cost zero.");
                        ci.OverrideEnergyCost = 0;
                    }
                    else
                    {
                        Console.WriteLine($"monkey paw had no card to reduce in value..");
                    }
                };
                ef.HandEffect.Add(thingie);
            }
        }
    }
}
