namespace StS
{
    public class HornCleat : Relic
    {
        public override string Name => nameof(HornCleat);
        private int RoundNumber { get; set; }

        public override void CardPlayed(Card card, EffectSet ef, IEntity player, IEntity enemy) { }
        public override void StartTurn(Player player, IEntity enemy, EffectSet relicEf)
        {
            RoundNumber++;
            if (RoundNumber == 2)
            {
                relicEf.PlayerEffect.AddBlockStep("HornCleat", 14);
            }
        }

        internal override Relic Copy()
        {
            return new HornCleat
            {
                RoundNumber = RoundNumber
            };
        }
    }
}
