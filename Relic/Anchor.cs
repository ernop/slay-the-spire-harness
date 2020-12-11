namespace StS
{
    public class Anchor : Relic
    {
        public override string Name => nameof(Anchor);
        internal int RoundNumber { get; set; }

        public override void StartTurn(Player player, IEntity enemy, EffectSet relicEf)
        {
            RoundNumber++;

            if (relicEf.SourceEffect.InitialBlock != 0)
            {
                throw new System.Exception("Shouldn't happen.");
            }

            if (RoundNumber == 1)
            {
                relicEf.SourceEffect.InitialBlock = 10;
            }
        }

        internal override Relic Copy()
        {
            return new Anchor
            {
                RoundNumber = RoundNumber
            };
        }
    }
}
