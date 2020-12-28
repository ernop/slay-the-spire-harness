namespace StS
{
    public class Frail : Status
    {
        public override string Name => nameof(Frail);

        public override StatusType StatusType => StatusType.Frail;

        public override bool NegativeStatus => true;

        internal override bool Scalable => false;

        internal override bool Permanent => false;

        internal override void CardWasPlayed(Card card, IndividualEffect playerSet, IndividualEffect enemySet, int intensity, bool statusIsTargeted, bool playerAction)
        {
            if (card.CardType == CardType.Skill && statusIsTargeted)
            {
                playerSet.AddBlockStep("Frail", 0.75, 10, additive: false);
            }
        }
    }
}
