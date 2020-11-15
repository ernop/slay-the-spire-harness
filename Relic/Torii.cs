namespace StS
{
    public class Torii : Relic
    {
        public override string Name => nameof(Torii);

        public override void CardPlayed(Card card, EffectSet ef, Entity player, Entity enemy)
        {
            if (card.CardType == CardType.Attack)
            {
                if (ef.TargetEffect.ReceiveDamage != null)
                {
                    ef.TargetEffect.ReceiveDamage.Add(
                        new Progression("ToriiReduction",
                            (el) =>
                                {
                                    if (el > 0 && el <= 5)
                                    {
                                        return 1;
                                    }
                                    return el;
                                }));
                }
            }
        }
    }
}
