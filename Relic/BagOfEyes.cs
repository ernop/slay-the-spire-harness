namespace StS
{
    public class BagOfEyes : Relic
    {
        public override string Name => throw new System.NotImplementedException();

        public override void FirstRoundStarts(Player player, Enemy enemy, EffectSet relicEf)
        {
            relicEf.TargetEffect.Status.Add(new StatusInstance(new Vulnerable(), 1));
        }

        public override void CardPlayed(Card card, EffectSet ef, Entity player, Entity enemy) { }
    }
}
