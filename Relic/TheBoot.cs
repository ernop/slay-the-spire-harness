using System;
using System.Linq;

namespace StS
{

    public class TheBoot : Relic
    {
        public override string Name => nameof(TheBoot);
        public override void CardPlayed(Card card, EffectSet ef, IEntity player, IEntity enemy)
        {
            if (card.CardType == CardType.Attack)
            {
                ef.EnemyEffect.DamageAdjustments.Add(new AttackProgression("TheBoot", (input) => input.Select(el => Math.Max(el, 5)).ToList(), 10));
            }
        }

        internal override Relic Copy()
        {
            return new TheBoot();
        }
    }
}
