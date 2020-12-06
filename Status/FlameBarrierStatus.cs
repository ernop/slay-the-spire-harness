using System.Linq;

namespace StS
{
    public class FlameBarrierStatus : Status
    {
        public override string Name => nameof(FlameBarrierStatus);

        public override StatusType StatusType => StatusType.FlameBarrierStatus;

        public override bool NegativeStatus => false;

        internal override bool Permanent => false;

        internal override bool Scalable => true;

        internal override void CardWasPlayed(Card card, IndividualEffect sourceSet, IndividualEffect targetSet, int intensity, bool statusIsTargeted, bool playerAction)
        {
            //triggers when player with this status is targeted.

            //we grab the progression from the damage pattern I'd receive, with default values of zero.
            //and this must be an enemy action.
            if (statusIsTargeted && !playerAction && card.CardType==CardType.Attack)
            {
                if (targetSet.InitialDamage == null)
                {
                    throw new System.Exception("How did you get here?");
                }
                else
                {
                    sourceSet.InitialDamage = targetSet.InitialDamage.Select(qq => intensity);
                }
            }
        }
    }
}
