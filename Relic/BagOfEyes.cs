namespace StS
{
    public class BagOfEyes : Relic
    {
        public override string Name => nameof(BagOfEyes);
        public override void StartFight(Deck d, EffectSet relicEf)
        {
            relicEf.EnemyEffect.Status.Add(new StatusInstance(new Vulnerable(), 1));
        }

        public override void CardPlayed(Card card, EffectSet ef, IEntity player, IEntity enemy) { }

        internal override Relic Copy()
        {
            return new BagOfEyes();
        }
    }
}
