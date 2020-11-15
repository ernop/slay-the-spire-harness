using System;

namespace StS
{
    public class Vulnerable : Status
    {
        public override StatusType StatusType => StatusType.Vulnerable;

        public override string Name => nameof(Vulnerable);

        internal override void Apply(Card card, IndividualEffect sourceSet, IndividualEffect targetSet, int intensity, bool statusIsTargeted)
        {
            if (card.CardType == CardType.Attack && statusIsTargeted)
            {
                targetSet.ReceiveDamage.Add(new Progression("VulnStatus",(el) =>
                {
                    if (el > 0)
                    {
                        return (int)Math.Floor(el * 1.5);
                    }
                    return 0;
                }));
            }
        }
    }
}
