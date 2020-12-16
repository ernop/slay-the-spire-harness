using System.Collections.Generic;

namespace StS
{
    public class SearingBlow : IroncladAttackCard
    {

        public override string Name => nameof(SearingBlow);
        public override int CiCanCallEnergyCost(int upgradeCount) => 2;

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
        {
            //TODO fix this.
            var dmg = 12 + 6 * upgradeCount;
            ef.EnemyEffect.InitialDamage = new List<int>() { dmg };
        }
    }
}
