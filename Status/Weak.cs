using System;

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
                targetSet.ReceiveDamage.Add(new Progression("Weak", (el) =>
                {
                    if (el > 0)
                    {
                        return (int)Math.Floor(el * 0.75);
                    }
                    return 0;
                }));
            }
        }
    }
}
