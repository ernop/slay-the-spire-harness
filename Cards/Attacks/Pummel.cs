using System.Collections.Generic;

namespace StS
{
    public class Pummel : IroncladAttackCard
    {
        public override string Name => nameof(Pummel);

        public override int CiCanCallEnergyCost(int upgradeCount) => 1;
        internal override bool Exhausts(int upgradeCount) => true;

        internal override void Play(EffectSet ef, IEntity source, IEntity target, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
        {
            var dmg = upgradeCount == 0 ? new List<int>() { 2, 2, 2, 2 } : new List<int>() { 2, 2, 2, 2, 2 };
            ef.TargetEffect.InitialDamage = dmg;
        }
    }
}
