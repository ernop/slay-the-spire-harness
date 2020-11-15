namespace StS
{

    public class Dexterity : Status
    {
        public override string Name => nameof(Dexterity);

        public override StatusType StatusType => StatusType.Dexterity;

        public override bool NegativeStatus => false;

        internal override bool Permanent => true;
        internal override bool Scalable => true;

        internal override void Apply(Card card, IndividualEffect sourceSet, IndividualEffect targetSet, int intensity, bool statusIsTargeted, bool playerAction)
        {
            //TODO: it would be better if this was a running total of defense so we could directly compare.
            if (card.CardType == CardType.Skill && statusIsTargeted)
            {
                targetSet.BlockAdjustments.Add(new Progression("DexEffect",
                    (el) =>
                    {
                        var blockGain = el + intensity;
                        if (blockGain >= 0)
                        {
                            return blockGain;
                        }
                        return 0;
                    }));
            }
        }
    }
}
