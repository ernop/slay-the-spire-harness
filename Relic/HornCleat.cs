namespace StS
{
    public class HornCleat : Relic
    {
        public override string Name => nameof(HornCleat);
        private int RoundNumber { get; set; }

        public override void CardPlayed(Card card, EffectSet ef, IEntity player, IEntity enemy) { }
        public override void StartTurn(Player player, IEnemy enemy, EffectSet relicEf)
        {
            RoundNumber++;
            if (RoundNumber == 2)
            {
                //Why am I sure I can override this?
                if (relicEf.SourceEffect.InitialBlock != 0)
                {
                    throw new System.Exception("Shouldn't happen.");
                }
                relicEf.SourceEffect.InitialBlock = 14;
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
