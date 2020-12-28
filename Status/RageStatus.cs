namespace StS
{
    public class RageStatus : Status
    {
        public override string Name => nameof(RageStatus);

        public override StatusType StatusType => StatusType.RageStatus;

        public override bool NegativeStatus => false;

        internal override bool Scalable => true;

        internal override bool Permanent => false;
        internal override void CardWasPlayed(Card card, IndividualEffect playerSet, IndividualEffect enemySet, int num, bool statusIsTargeted, bool playerAction)
        {
            if (card.CardType == CardType.Attack && playerAction)
            {
                //TODO this is not exactly right; e.g. what if we double play a card?
                playerSet.AddBlockStep("Rage", num);
            }
        }
    }
}
