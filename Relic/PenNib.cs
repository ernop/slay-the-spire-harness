namespace StS
{
    public class PenNib : Relic
    {
        public override string Name => nameof(PenNib);
        public int AttackCount { get; set; } = 0;
        public override void CardPlayed(Card card, EffectSet ef, IEntity player, IEntity enemy)
        {
            if (card.CardType == CardType.Attack)
            {
                AttackCount = (AttackCount + 1) % 10;
                if (AttackCount == 9)
                {
                    ef.SourceEffect.Status.Add(new StatusInstance(new PenNibStatus(), 1));
                }
            }
        }

        internal override Relic Copy() => new PenNib
        {
            AttackCount = AttackCount
        };

        public override string ToString()
        {
            return $"PenNib:{AttackCount}";
        }
    }
}
