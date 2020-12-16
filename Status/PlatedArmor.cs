namespace StS
{
    /// <summary>
    /// This needs to notice when the player receives attack damage.
    /// </summary>
    public class PlatedArmor : Status
    {


        private Entity Entity { get; set; }

        public override void Apply(Fight f, Deck d, Entity e)
        {
            e.TakeDamage += PlatedArmorAttackedAndTookDamage;
            Entity = e;
        }

        public override void Unapply(Fight f, Deck d, Entity e)
        {
            e.TakeDamage -= PlatedArmorAttackedAndTookDamage;
            Entity = null;
        }

        public override string Name => nameof(PlatedArmor);

        public override StatusType StatusType => StatusType.PlatedArmor;

        public override bool NegativeStatus => false;
        public override bool CanAddNegative => false;

        internal override bool Scalable => true;

        internal override bool Permanent => true;

        /// <summary>
        /// Todo this is still kinda messy
        /// </summary>
        internal override void StatusStartTurn(Entity parent, StatusInstance instance, IndividualEffect statusHolderIe, IndividualEffect otherId)
        {
            statusHolderIe.InitialBlock = instance.Intensity;
        }

        private void PlatedArmorAttackedAndTookDamage(EffectSet ef, int damageAmount, CardInstance ci)
        {
            if (ci.Card.CardType == CardType.Attack && damageAmount > 0)
            {
                ef.PlayerEffect.Status.Add(new StatusInstance(new PlatedArmor(), -1));
            }
        }
    }
}
