using System.Collections.Generic;

namespace StS
{
    public class Strike : IroncladAttackCard
    {
        public override string Name => nameof(Strike);
        public override int CiCanCallEnergyCost(int upgradeCount) => 1;

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
        {
            int dmg = upgradeCount==0?6:9;
            ef.EnemyEffect.SetInitialDamage(dmg);
        }
    }
}
