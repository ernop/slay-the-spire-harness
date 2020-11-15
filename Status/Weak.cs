using System;
using System.Linq;

namespace StS
{
    public class Weak : Status
    {
        public override StatusType StatusType => StatusType.Weak;

        public override string Name => nameof(Weak);

        public override bool NegativeStatus => true;

        public override bool Permanent => false;

        internal override void Apply(Card card, IndividualEffect sourceSet, IndividualEffect targetSet, int intensity, bool statusIsTargeted)
        {
            if (card.CardType == CardType.Attack && !statusIsTargeted)
            {
                targetSet.DamageAdjustments.Add(new AttackProgression("Weak", (el) => el.Select(qq => qq > 0 ? (int)Math.Floor(qq * 0.75) : 0).ToList()));
            }
        }
    }
}
