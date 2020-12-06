namespace StS
{
    public class Turnip : Relic
    {
        public override string Name => nameof(Turnip);


        internal override Relic Copy()
        {
            return new Turnip();
        }
    }
}
