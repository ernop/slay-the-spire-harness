namespace StS
{
    public class HornCleat : Relic
    {
        public override string Name => nameof(HornCleat);
        private int RoundNumber { get; set; }

        public override void CardPlayed(Card card, EffectSet ef, Entity player, Entity enemy) { }
        public override void EndTurn(Player player, Enemy enemy, EffectSet relicEf)
        {
            RoundNumber++;
            //RoundNumber just ended; we are going into RoundNumber+1
            if (RoundNumber == 1)
            {
                //Why am I sure I can override this?
                if (relicEf.SourceEffect.InitialBlock != 0)
                {
                    throw new System.Exception("Shouldn't happen.");
                }
                relicEf.SourceEffect.InitialBlock = 14;
            }
        }
    }
}
