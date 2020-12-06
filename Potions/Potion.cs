namespace StS
{
    public abstract class Potion
    {
        public abstract void Apply(Fight f, Player player, Enemy enemy);

        internal abstract Potion Copy();

    }
}
