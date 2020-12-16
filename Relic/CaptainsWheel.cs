namespace StS
{
    public class CaptainsWheel : Relic
    {
        public override string Name => nameof(CaptainsWheel);
        internal int RoundNumber { get; set; }

        public override void StartTurn(Player player, IEntity enemy, EffectSet relicEf)
        {
            RoundNumber++;

            if (relicEf.PlayerEffect.InitialBlock != 0)
            {
                throw new System.Exception("Shouldn't happen.");
            }

            if (RoundNumber == 3)
            {
                relicEf.PlayerEffect.InitialBlock = 18;
            }
        }

        internal override Relic Copy()
        {
            return new CaptainsWheel
            {
                RoundNumber = RoundNumber
            };
        }
    }
}
