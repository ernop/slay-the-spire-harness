using System;
using System.Linq;

namespace StS
{
    public class Vulnerable : Status
    {
        public override StatusType StatusType => StatusType.Vulnerable;

        public override string Name => nameof(Vulnerable);

        public override bool NegativeStatus => true;

        public override bool Permanent => false;

        internal override void Apply(Card card, IndividualEffect sourceSet, IndividualEffect targetSet, int intensity, bool statusIsTargeted)
        {
            if (card.CardType == CardType.Attack && statusIsTargeted)
            {
                targetSet.DamageAdjustments.Add(new AttackProgression("Vuln", (el) => el.Select(qq => qq > 0 ? (int)Math.Floor(qq * 1.5) : 0).ToList()));
            }
        }
    }
}
