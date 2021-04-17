using System.Collections.Generic;

namespace StS
{
    public class Intimidate : IroncladSkillCard
    {
        public override string Name => nameof(Intimidate);
        public override TargetType TargetType => TargetType.Enemy;
        public override EnergyCostInt CiCanCallEnergyCost(int upgradeCount) => new EnergyCostInt(0);
        internal override bool Exhausts(int UpgradeCount) => true;

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, long? key = null)
        {
            var weak = upgradeCount == 0 ? 1 : 2;
            ef.EnemyEffect.Status.Add(new StatusInstance(new Weak(), weak));
        }
    }
}
