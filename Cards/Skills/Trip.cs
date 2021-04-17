using System.Collections.Generic;

using static StS.Helpers;

namespace StS
{
    public class Trip : SkillCard
    {
        public override string Name => nameof(Trip);

        public override CardDomain CardDomain => CardDomain.Colorless;

        public override TargetType TargetType => TargetType.Enemy;

        public override EnergyCostInt CiCanCallEnergyCost(int upgradeCount) => new EnergyCostInt(0);

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, long? key = null)
        {
            ef.EnemyEffect.Status.AddRange(GSS(new Vulnerable(), 2));
        }
    }
}
