using System.Linq;

namespace StS
{
    public class Strength : Status
    {
        public override string Name => nameof(Strength);

        public override StatusType StatusType => StatusType.Strength;

        public override bool NegativeStatus => false;

        internal override bool Permanent => true;
        internal override bool Scalable => true;

        internal override void CardWasPlayed(Card card, IndividualEffect sourceSet, IndividualEffect targetSet, int intensity, bool statusIsTargeted, bool playerAction)
        {
            //statusIsTargeted means the target has the status.
            //in this case we only care if the subject has the status.
            if (card.CardType == CardType.Attack && !statusIsTargeted)
            {
                if (card.Name == "HeavyBlade")
                {
                    //already calculated.
                    return;
                }
                if (targetSet.InitialDamage == null)
                {
                    throw new System.Exception("Why am i calculating strength without having a base damage?");
                }

                //strength always calculated immediately after initial damage.

                //this should be genericized; here it's assuming strength only hits enemy when actually it hits whoever the target is.
                targetSet.DamageAdjustments.Insert(0, new AttackProgression("StrengthStatus", (el) => el.Select(qq => qq + intensity).ToList()));

            }
        }
    }
}
