namespace StS
{
    public class CaptainsWheel : Relic
    {
        public override string Name => nameof(CaptainsWheel);
        internal int RoundNumber { get; set; }

        public override void StartTurn(Player player, IEnemy enemy, EffectSet relicEf)
        {
            RoundNumber++;

            if (relicEf.SourceEffect.InitialBlock != 0)
            {
                throw new System.Exception("Shouldn't happen.");
            }

            if (RoundNumber == 3)
            {
                relicEf.SourceEffect.InitialBlock = 18;
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
