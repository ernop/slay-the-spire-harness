namespace StS
{
    public class Vajra : Relic
    {
        internal override Relic Copy() => new Vajra();
        public override string Name => nameof(Vajra);
        public override void StartFight(Deck d, EffectSet ef)
        {
            ef.PlayerEffect.Status.Add(new StatusInstance(new Strength(), 1));
        }
    }
}
