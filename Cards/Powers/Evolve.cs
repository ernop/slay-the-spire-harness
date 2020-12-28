using System.Collections.Generic;

using static StS.Helpers;

namespace StS
{
    public class Evolve : IroncladPowerCard
    {
        public override string Name => nameof(Evolve);

        public override int CiCanCallEnergyCost(int upgradeCount) => 1;

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, int? key = null)
        {
            ef.PlayerEffect.Status.AddRange(GSS(new EvolveStatus(), upgradeCount == 0 ? 1 : 2));
        }
    }
}
