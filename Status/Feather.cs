namespace StS
{
    public class Feather : Status
    {
        public override string Name => nameof(Feather);

        public override StatusType StatusType => StatusType.Feather;

        public override bool NegativeStatus => false;

        internal override bool Scalable => true;

        internal override bool Permanent => true;

        internal override void Apply(Card card, IndividualEffect sourceSet, IndividualEffect targetSet, int num, bool statusIsTargeted, bool playerAction)
        {
        }

        internal override void EndTurn(Entity parent, StatusInstance instance, EffectSet endTurnEf)
        {
            endTurnEf.SourceEffect.Status.Add(new StatusInstance(new Strength(), instance.Intensity));
        }
    }
}
