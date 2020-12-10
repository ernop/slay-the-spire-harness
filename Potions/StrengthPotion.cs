namespace StS
{
    public class StrengthPotion : Potion
    {
        public override void Apply(Fight f, Player player, Enemy enemy, EffectSet ef)
        {
            var si = new StatusInstance(new Strength(), 2);
            ef.TargetEffect.Status.Add(si);
        }

        public override bool SelfTarget() => true;

        internal override Potion Copy() => new StrengthPotion();
    }
}
