namespace StS
{
    /// <summary>
    /// The one used by the pillbugs where they get damage when you attack them.
    /// </summary>
    public class Aggressive : Status
    {

        public override string Name => nameof(Aggressive);

        public override StatusType StatusType => StatusType.Aggressive;

        public override bool NegativeStatus => false;

        /// <summary>
        /// permanent until it self-removes.
        /// </summary>
        internal override bool Permanent => true;

        internal override bool Scalable => true;

        /// <summary>
        /// someone played a card against an entity with aggro status.  Is it safe to assume I'm always target here as far as ef is concerned?
        /// Question: someone playing strike with -6 strength; does it trigger aggressive?  probably shouldn't.
        /// </summary>
        internal override void Apply(Card card, IndividualEffect sourceSet, IndividualEffect targetSet, int intensity, bool statusIsTargeted, bool playerAction)
        {
            if (card.CardType == CardType.Attack && statusIsTargeted)
            {
                if (intensity > 0)
                {
                    //actually this should be a check of the value.
                    if (targetSet.DamageAdjustments != null)
                    {
                        //bit weird to hack intensity this wasy.
                        var negativeAggressiveStatus = new StatusInstance(new Aggressive(), -1 * intensity);

                        //whoah, this will be applied when the attack is actually resolved.
                        //since here we're 
                        targetSet.Status.Add(negativeAggressiveStatus);

                        targetSet.BlockAdjustments.Add(new Progression("AggroStatus", (el, entity) => intensity));
                    }
                }
            }
        }
    }
}
