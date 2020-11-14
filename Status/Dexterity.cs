namespace StS
{
    public class Dexterity : Status
    {
        public override string Name => nameof(Dexterity);

        public override StatusType StatusType => StatusType.Dexterity;

        internal override void Apply(Card card, EffectSet set, int intensity)
        {
            //you will have an old pgb like (_) = 5;
            if (card.CardType == CardType.Skill)
            {
                set.PlayerGainBlock.Add(
                    (el) =>
                    {
                        if (el > 0)
                        {
                            return el + intensity;
                        }
                        return 0;
                    });
            }
        }
    }
}
