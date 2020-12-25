namespace StS
{
    public class StrengthPotion : Potion
    {
        public override string Name => nameof(StrengthPotion);

        public override void Apply(Fight f, Player player, IEnemy enemy, EffectSet ef)
        {
            var si = new StatusInstance(new Strength(), 2);
            ef.PlayerEffect.Status.Add(si);
        }

        public override bool SelfTarget => true;

        internal override Potion Copy() => new StrengthPotion();
    }
}
