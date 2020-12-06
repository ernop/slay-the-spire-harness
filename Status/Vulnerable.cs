using System;
using System.Linq;

namespace StS
{
    public class Vulnerable : Status
    {
        public override StatusType StatusType => StatusType.Vulnerable;

        public override string Name => nameof(Vulnerable);

        public override bool NegativeStatus => true;

        internal override bool Permanent => false;
        internal override bool Scalable => false;

        internal override void CardWasPlayed(Card card, IndividualEffect sourceSet, IndividualEffect targetSet, int intensity, bool statusIsTargeted, bool playerAction)
        {
            if (card.CardType == CardType.Attack && statusIsTargeted)
            {
                targetSet.DamageAdjustments.Add(new AttackProgression("Vuln", (el) => el.Select(qq => qq > 0 ? qq * 1.5 : 0).ToList()));
            }
        }
    }
}
