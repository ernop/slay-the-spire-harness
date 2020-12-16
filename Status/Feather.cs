namespace StS
{
    public class Feather : Status
    {
        public override string Name => nameof(Feather);

        public override StatusType StatusType => StatusType.Feather;

        public override bool NegativeStatus => false;

        internal override bool Scalable => true;

        internal override bool Permanent => true;

        internal override void CardWasPlayed(Card card, IndividualEffect playerSet, IndividualEffect enemySet, int intensity, bool statusIsTargeted, bool playerAction)
        {
        }

        internal override void StatusEndTurn(Entity parent, StatusInstance instance, IndividualEffect statusHolderIe, IndividualEffect otherIe)
        {
            statusHolderIe.Status.Add(new StatusInstance(new Strength(), instance.Intensity));
        }
    }
}
