using System.Collections.Generic;

namespace StS
{
    public class TwinStrike : IroncladAttackCard
    {
        public override string Name => nameof(TwinStrike);

        public override EnergyCostInt CiCanCallEnergyCost(int upgradeCount) => new EnergyCostInt(1);

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, long? key = null)
        {
            var dmg = upgradeCount == 0 ? 5 : 7;
            ef.EnemyEffect.SetInitialDamage(dmg, dmg);
        }
    }
}
