using System.Collections.Generic;

namespace StS
{
    public class Uppercut : IroncladAttackCard
    {
        public override string Name => nameof(Uppercut);
        public override int CiCanCallEnergyCost(int upgradeCount) => 2;

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, int? key = null)
        {
            int weak;
            int vuln;
            if (upgradeCount == 0)
            {
                weak = 1;
                vuln = 1;
            }
            else
            {
                weak = 2;
                vuln = 2;
            }
            ef.EnemyEffect.SetInitialDamage(13);
            ef.EnemyEffect.Status.Add(new StatusInstance(new Vulnerable(), vuln));
            ef.EnemyEffect.Status.Add(new StatusInstance(new Weak(), weak));
        }
    }
}
