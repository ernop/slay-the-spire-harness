using System.Collections.Generic;
using System.Linq;

namespace StS
{
    public class Vajra : Relic
    {
        public int Intensity { get; set; } = 0;

        public override string Name => nameof(Vajra);

        public override void CardPlayed(Card card, EffectSet ef, Entity player, Entity enemy)
        {
            if (Intensity > 0 && card.CardType==CardType.Attack)
            {
                ef.TargetEffect.DamageAdjustments.Add(new AttackProgression("VajraEffect", (el) => el.Select(qq => qq + Intensity).ToList()));
            }
            
        }

        public override string ToString()
        {
            return $"Vajra:{Intensity}";
        }
    }
}
