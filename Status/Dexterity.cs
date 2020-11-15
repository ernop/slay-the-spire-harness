namespace StS
{

    public class Dexterity : Status
    {
        public override string Name => nameof(Dexterity);

        public override StatusType StatusType => StatusType.Dexterity;

        public override bool NegativeStatus => false;

        /// <summary>
        /// Intensity and duration are kind of an overload.
        /// It's actually if(permanent) {intensity implies strength} else {intensity implied length of status}
        /// </summary>
        public override bool Permanent => true;

        internal override void Apply(Card card, IndividualEffect sourceSet, IndividualEffect targetSet, int intensity, bool statusIsTargeted)
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
