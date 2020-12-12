using System.Collections.Generic;

using static StS.Helpers;

namespace StS
{
    public class Evolve : IroncladPowerCard
    {
        public override string Name => nameof(Evolve);

        public override int CiCanCallEnergyCost(int upgradeCount) => 1;

        internal override void Play(EffectSet ef, IEntity source, IEntity target, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
        {
            ef.TargetEffect.Status.AddRange(GetStatuses(new EvolveStatus(), upgradeCount == 0 ? 1 : 2));
        }
    }
}
