using System.Linq;

namespace StS
{

    public class Dexterity : Status
    {
        public override string Name => nameof(Dexterity);

        public override StatusType StatusType => StatusType.Dexterity;

        public override bool NegativeStatus => false;

        internal override bool Permanent => true;
        internal override bool Scalable => true;

        internal override void CardWasPlayed(Card card, IndividualEffect playerSet, IndividualEffect enemySet, int intensity, bool statusIsTargeted, bool playerAction)
        {
            //TODO: it would be better if this was a running total of defense so we could directly compare.
            //only add block if it's actually a block card (not entrench)
            if (card.CardType == CardType.Skill && statusIsTargeted)
            {
                //dex only adds block if there is an existing additive.
                //i.e. entrench doesn't quality.
                foreach (var existingSteps in playerSet.GetBlockActions.Where(el=>el.Order<10))
                {
                    if (existingSteps.Additive)
                    {
                        playerSet.AddBlockStep("Dex", intensity);
                        break;
                    }
                }                
            }
        }
    }
}
