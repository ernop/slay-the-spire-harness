namespace StS
{
    public class EssenceOfSteel : Potion
    {
        public override string Name => nameof(EssenceOfSteel);

        public override bool SelfTarget() => true;

        public override void Apply(Fight fight, Player player, IEnemy enemy, EffectSet ef)
        {
            var si = new StatusInstance(new PlatedArmor(), 4);
            ef.PlayerEffect.Status.Add(si);
        }

        internal override Potion Copy() => new EssenceOfSteel();
    }
}
