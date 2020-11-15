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
                ef.TargetEffect.ReceiveDamage.Add(new Progression("VajraEffect", (el) =>
                {
                    if (el > 0)
                    {
                        return el + Intensity;
                    }
                    return 0;
                }));
            }
            
        }

        public override string ToString()
        {
            return $"Vajra:{Intensity}";
        }
    }
}
