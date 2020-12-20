
using System.Linq;

using static StS.Helpers;

namespace StS
{
    public class FlameBarrierStatus : Status
    {
        public override string Name => nameof(FlameBarrierStatus);

        public override StatusType StatusType => StatusType.FlameBarrierStatus;

        public override bool NegativeStatus => false;

        internal override bool Permanent => true;

        internal override bool Scalable => true;
        private Entity Entity { get; set; }

        public override void Apply(Fight f, Deck d, Entity e)
        {
            e.BeAttacked += AttackResponse;
            Entity = e;
        }

        public override void Unapply(Fight f, Deck d, Entity e)
        {
            e.BeAttacked += AttackResponse;
            Entity = null;
        }

        private void AttackResponse(EffectSet ef)
        {
            var si = Entity.StatusInstances.Single(el => el.Status.Name == nameof(FlameBarrierStatus));
            //triggers when player with this status is targeted.

            //we grab the progression from the damage pattern I'd receive, with default values of zero.
            //and this must be an enemy action.

            var th = Repeat(si.Intensity, ef.PlayerEffect.AttackCount);
            ef.EnemyEffect.SetInitialDamage(th.ToArray());
        }
    }
}
