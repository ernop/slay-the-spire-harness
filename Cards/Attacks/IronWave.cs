using System.Collections.Generic;

namespace StS
{
    public class IronWave : IroncladAttackCard
    {
        public override string Name => nameof(IronWave);
        public override EnergyCostInt CiCanCallEnergyCost(int upgradeCount) => new EnergyCostInt(1);
        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, long? key = null)
        {
            int dmg;
            int block;
            if (upgradeCount == 0)
            {
                dmg = 5;
                block = 5;
            }
            else
            {
                dmg = 7;
                block = 7;
            }

            ef.EnemyEffect.SetInitialDamage(dmg);
            ef.PlayerEffect.AddBlockStep("Ironwave", block);
        }
    }
}
