namespace StS
{
    public abstract class Potion
    {
        public abstract void Apply(Player player, Enemy enemy);

        internal abstract Potion Copy();

    }

    public class EssenceOfSteel : Potion
    {
        public override void Apply(Player player, Enemy enemy)
        {
            var s = new StatusInstance(new PlatedArmor(player), 4);
            player.ApplyStatus(s);
        }

        internal override Potion Copy() => new EssenceOfSteel();
    }
}
