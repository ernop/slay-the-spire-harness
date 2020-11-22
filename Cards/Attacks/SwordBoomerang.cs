using System.Collections.Generic;

namespace StS
{
    public class SwordBoomerang : IroncladAttackCard
    {
        public override string Name => nameof(SwordBoomerang);

        public override int CiCanCallEnergyCost(int upgradeCount) => 1;

        internal override void Play(EffectSet ef, Entity source, Entity target, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
        {
            if (upgradeCount == 0)
            {
                ef.TargetEffect.InitialDamage = new List<int>() { 3, 3, 3 };
            }
            else
            {
                ef.TargetEffect.InitialDamage = new List<int>() { 3, 3, 3, 3 };
            }
        }
    }
}
