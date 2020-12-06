using System.Linq;

namespace StS
{
    /// <summary>
    /// This needs to notice when the player receives attack damage.
    /// </summary>
    public class PlatedArmor : Status
    {
        private void PlatedArmorTookDamage(Entity e)
        {
            var si = e.StatusInstances.SingleOrDefault(el => el.Status.StatusType == StatusType.PlatedArmor);
            if (si == null)
            {
                throw new System.Exception("Event should have been removed already");
                return;
            }
            si.Intensity--;
            if (si.Intensity < 0)
            {
                si.Intensity = 0;
            }
        }

        public override void Apply(Deck d, Entity e)
        {
            e.TakeDamage += PlatedArmorTookDamage;
        }

        public override void Unapply(Deck d, Entity e)
        {
            e.TakeDamage -= PlatedArmorTookDamage;
        }

        public override string Name => nameof(PlatedArmor);

        public override StatusType StatusType => StatusType.PlatedArmor;

        public override bool NegativeStatus => false;

        internal override bool Scalable => true;

        internal override bool Permanent => true;

        /// <summary>
        /// Todo this is still kinda messy
        /// </summary>
        internal override void StartTurn(Entity parent, StatusInstance instance, EffectSet endTurnEf)
        {
            endTurnEf.SourceEffect.InitialBlock = instance.Intensity;
        }
    }
}
