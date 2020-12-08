namespace StS
{
    public class EssenceOfSteel : Potion
    {
        public override bool SelfTarget() => true;

        public override void Apply(Fight fight, Player player, Enemy enemy, EffectSet ef)
        {
            var si = new StatusInstance(new PlatedArmor(), 4);
            ef.TargetEffect.Status.Add(si);
        }

        internal override Potion Copy() => new EssenceOfSteel();
    }
}
