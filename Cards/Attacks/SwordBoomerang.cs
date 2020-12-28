using System.Collections.Generic;

namespace StS
{

    public class SwordBoomerang : IroncladAttackCard
    {
        public override string Name => nameof(SwordBoomerang);

        public override int CiCanCallEnergyCost(int upgradeCount) => 1;

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, int? key = null)
        {
            if (upgradeCount == 0)
            {
                ef.EnemyEffect.SetInitialDamage(3, 3, 3);
            }
            else
            {
                ef.EnemyEffect.SetInitialDamage(3, 3, 3,3);
            }
        }
    }
}
