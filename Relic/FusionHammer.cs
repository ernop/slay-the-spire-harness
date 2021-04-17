namespace StS
{
    /// <summary>
    /// Todo: blocks upgrading cards at campfires.
    /// </summary>
    public class FusionHammer : Relic
    {
        public override string Name => nameof(FusionHammer);

        public override void CardPlayed(Card card, EffectSet ef, IEntity player, IEntity enemy) { }

        internal override Relic Copy() => new FusionHammer();

        public override bool ExtraEnergy => true;
    }
}
