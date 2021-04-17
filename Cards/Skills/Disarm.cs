using System.Collections.Generic;

namespace StS
{
    public class Disarm : IroncladSkillCard
    {
        public override string Name => nameof(Disarm);

        public override TargetType TargetType => TargetType.Enemy;
        public override EnergyCostInt CiCanCallEnergyCost(int upgradeCount) => new EnergyCostInt(1);
        internal override bool Exhausts(int upgradeCount) => true;

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, long? key = null)
        {
            int amt = upgradeCount == 0 ? -2 : -3;

            ef.EnemyEffect.Status.Add(new StatusInstance(new Strength(), amt));
        }
    }
}
