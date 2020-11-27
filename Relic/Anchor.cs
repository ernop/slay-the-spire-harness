namespace StS
{
    public class Anchor : Relic
    {
        public override string Name => nameof(Anchor);
        private bool Used { get; set; }
        private int RoundNumber { get; set; }

        public override void CardPlayed(Card card, EffectSet ef, Entity player, Entity enemy) { }
        public override void StartTurn(Player player, Enemy enemy, EffectSet relicEf)
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
    }
}
