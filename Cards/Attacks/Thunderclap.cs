using System.Collections.Generic;

namespace StS
{
    public class Thunderclap : IroncladAttackCard
    {
        public override string Name => nameof(Thunderclap);

        public override EnergyCostInt CiCanCallEnergyCost(int upgradeCount) => new EnergyCostInt(1);

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, long? key = null)
        {
            var vuln = upgradeCount == 0 ? 1 : 2;
            var dmg = upgradeCount == 0 ? 4 : 7;
            ef.EnemyEffect.SetInitialDamage(dmg);
            ef.EnemyEffect.Status.Add(new StatusInstance(new Vulnerable(), vuln));
        }
    }
}
