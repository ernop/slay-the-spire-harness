using System.Collections.Generic;

namespace StS
{
    public class BodySlam : IroncladAttackCard
    {
        public override string Name => nameof(BodySlam);


        public override int CiCanCallEnergyCost(int upgradeCount) => upgradeCount == 0 ? 1 : 0;

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, long? key = null)
        {
            ef.EnemyEffect.SetInitialDamage(player.Block);
        }
    }
}
