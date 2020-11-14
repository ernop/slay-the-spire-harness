namespace StS
{
    public class RelicInstance : Relic
    {
        public Relic Relic { get; set; }
        public RelicInstance(Relic relic)
        {
            Relic = relic;
        }

        public override EffectSet CardPlayed(Card card)
        {
            Relic.CardPlayed(card);
            return null;
        }
    }
}
