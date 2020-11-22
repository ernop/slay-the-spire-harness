using System.Collections.Generic;

namespace StS
{
    public class SearingBlow : IroncladAttackCard
    {

        public override string Name => nameof(SearingBlow);
        public override int CiCanCallEnergyCost(int upgradeCount) => 2;

        internal override void Play(EffectSet ef, Entity source, Entity target, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
        {
            var dmg = 12 + 6 * upgradeCount;
            ef.TargetEffect.InitialDamage = new List<int>() { dmg };
        }
    }
}
