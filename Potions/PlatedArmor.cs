using System.Linq;

namespace StS
{
    /// <summary>
    /// This needs to notice when the player receives attack damage.
    /// </summary>
    public class PlatedArmor : Status
    {
        public PlatedArmor(Entity e)
        {
            //in tests, null entity.
            if (e == null)
            {
                return;
            }
            e.TakeDamage += NotifyStatusOfDamage;
        }

        private void NotifyStatusOfDamage(Entity e)
        {
            var si = e.StatusInstances.SingleOrDefault(el => el.Status.StatusType == StatusType.PlatedArmor);
            if (si == null)
            {
                return;
            }
            si.Intensity--;
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
