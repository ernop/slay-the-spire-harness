namespace StS
{
    public class FusionHammer : Relic
    {
        public override string Name => nameof(FusionHammer);

        public override void CardPlayed(Card card, EffectSet ef, Entity player, Entity enemy) { }
        public override bool ExtraEnergy => true;
    }
}
