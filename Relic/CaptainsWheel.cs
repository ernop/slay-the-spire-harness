namespace StS
{
    public class CaptainsWheel : Relic
    {
        public override string Name => nameof(CaptainsWheel);
        internal int RoundNumber { get; set; }

        public override void StartTurn(Player player, IEntity enemy, EffectSet relicEf)
        {
            RoundNumber++;
          
            if (RoundNumber == 3)
            {
                relicEf.PlayerEffect.AddBlockStep("Wheel", 18);
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
