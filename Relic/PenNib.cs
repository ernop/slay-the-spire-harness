using System;
using System.Text;

namespace StS
{
    public class PenNib : Relic
    {
        public override string Name => nameof(PenNib);
        public int AttackCount { get; set; } = 0;
        public override void CardPlayed(Card card, EffectSet ef, Entity player, Entity enemy)
        {
            if (card.CardType == CardType.Attack)
            {
                AttackCount = (AttackCount + 1) % 10;
                if (AttackCount == 9)
                {
                    ef.SourceEffect.Status.Add(new StatusInstance(new PenNibDoubleDamage(), 1, int.MaxValue));
                }
            }
        }

        public override string ToString()
        {
            return $"PenNib:{AttackCount}";
        }
    }
}
