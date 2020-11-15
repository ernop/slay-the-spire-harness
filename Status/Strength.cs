namespace StS
{
    public class Strength : Status
    {
        public override string Name => nameof(Strength);

        public override StatusType StatusType => StatusType.Strength;

        internal override void Apply(Card card, IndividualEffect sourceSet, IndividualEffect targetSet, int intensity, bool statusIsTargeted)
        {
            //statusIsTargeted means thee target has the status.
            //in this case we only care if the subject has the status.
            if (card.CardType == CardType.Attack && !statusIsTargeted)
            {
                if (targetSet.ReceiveDamage.Count==0)
                {
                    throw new System.Exception("Why am i calculating strength without having a base damage?");
                }

                //strength always calculated immediately after initial damage.

                //this should be genericized; here it's assuming strength only hits enemy when actually it hits whoever the target is.
                targetSet.ReceiveDamage.Insert(1, new Progression("StrengthStatus", (el) =>
                {
                    if (el > 0)
                    {
                        return el + intensity;
                    }
                    return 0;
                }));
            }            
        }
    }
}
