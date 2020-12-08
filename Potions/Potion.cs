namespace StS
{
    public abstract class Potion
    {
        public abstract void Apply(Fight f, Player player, Enemy enemy, EffectSet ef);
        public abstract bool SelfTarget();

        internal abstract Potion Copy();

    }
}
