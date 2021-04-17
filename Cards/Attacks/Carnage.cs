using System.Collections.Generic;

namespace StS
{
    public class Carnage : IroncladAttackCard
    {
        public override string Name => nameof(Carnage);

        public override EnergyCostInt CiCanCallEnergyCost(int upgradeCount) => new EnergyCostInt(2);
        internal override bool Ethereal(int upgradeCount) => true;

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, long? key = null)
        {
            var dmg = upgradeCount == 0 ? 20 : 28;
            ef.EnemyEffect.SetInitialDamage(dmg);
        }
    }
}
