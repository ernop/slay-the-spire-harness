using System.Collections.Generic;

namespace StS
{
    public class Shockwave : IroncladSkillCard
    {
        public override string Name => nameof(Shockwave);

        public override TargetType TargetType => TargetType.Enemy;

        public override int CiCanCallEnergyCost(int upgradeCount) => 2;

        internal override bool Exhausts(int upgradeCount) => true;
        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, int? key = null)
        {
            var vuln = upgradeCount == 0 ? 3 : 5;
            var weak = upgradeCount == 0 ? 3 : 5;
            ef.EnemyEffect.Status.Add(new StatusInstance(new Vulnerable(), vuln));
            ef.EnemyEffect.Status.Add(new StatusInstance(new Weak(), weak));
        }
    }
}
