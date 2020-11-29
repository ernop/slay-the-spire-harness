namespace StS
{
    public class BagOfEyes : Relic
    {
        public override string Name => throw new System.NotImplementedException();
        private int RoundNumber { get; set; }
        public override void StartTurn(Player player, IEnemy enemy, EffectSet relicEf)
        {
            RoundNumber++;
            if (RoundNumber == 1)
            {
                relicEf.TargetEffect.Status.Add(new StatusInstance(new Vulnerable(), 1));
            }
        }

        public override void CardPlayed(Card card, EffectSet ef, IEntity player, IEntity enemy) { }

        internal override Relic Copy()
        {
            return new BagOfEyes
            {
                RoundNumber = RoundNumber
            };
        }
    }
}
