using System.Collections.Generic;

namespace StS
{
    public class Intimidate : IroncladAttackCard
    {
        public override string Name => nameof(Intimidate);

        public override int CiCanCallEnergyCost(int upgradeCount) => 0;
        internal override bool Exhausts(int UpgradeCount) => true;

        internal override void Play(EffectSet ef, IEntity source, IEntity target, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
        {
            var week = upgradeCount == 0 ? 1 : 2;
            ef.TargetEffect.Status.Add(new StatusInstance(new Weak(), week));
        }
    }
}
