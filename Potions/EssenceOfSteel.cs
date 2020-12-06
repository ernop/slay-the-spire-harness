namespace StS
{
    public class EssenceOfSteel : Potion
    {
        public override void Apply(Fight fight, Player player, Enemy enemy)
        {
            var si = new StatusInstance(new PlatedArmor(), 4);
            fight.ApplyStatus(player, si);
        }

        internal override Potion Copy() => new EssenceOfSteel();
    }
}
