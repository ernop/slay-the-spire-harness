namespace StS
{
    public class BustedCrown : Relic
    {
        public override string Name => nameof(BustedCrown);

        public override void CardPlayed(Card card, EffectSet ef, IEntity player, IEntity enemy) { }

        internal override Relic Copy() => new BustedCrown();
        public override bool ExtraEnergy => true;
    }
}
