using System;
using System.Text;

namespace StS
{
    public class PenNib : Relic
    {
        public int AttackCount { get; set; } = 0;
        public override void CardPlayed(Card card, EffectSet ef)
        {
            if (card.CardType == CardType.Attack)
            {
                AttackCount = (AttackCount + 1) % 10;
                if (AttackCount == 9)
                {
                    ef.PlayerStatus.Add(new StatusInstance(new PenNibDoubleDamage(), 1, int.MaxValue));
                }
            }
        }

        public override void FightStarted()
        {
            throw new NotImplementedException();
        }

        public override void NewTurn()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"PenNib:{AttackCount}";
        }
    }
}
