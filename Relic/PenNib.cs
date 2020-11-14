using System;
using System.Text;

namespace StS
{
    public class PenNib : Relic
    {
        public int AttackCount { get; set; } = 0;
        public override EffectSet CardPlayed(Card card)
        {
            var es = new EffectSet();
            if (card.CardType == CardType.Attack)
            {
                AttackCount = (AttackCount + 1) % 10;
                es.PlayerStatus.Add(new StatusInstance(new PinNibDoubleDamage(),int.MaxValue, int.MaxValue));
            }
            return es;
        }
    }
}
