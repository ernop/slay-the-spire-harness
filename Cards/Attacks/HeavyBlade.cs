using System.Collections.Generic;
using System.Linq;

namespace StS
{
    public class HeavyBlade : IroncladAttackCard
    {
        public override string Name => nameof(HeavyBlade);

        public override int CiCanCallEnergyCost(int upgradeCount) => 2;
        internal override void Play(EffectSet ef, Entity source, Entity target, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
        {
            var str = source.StatusInstances.SingleOrDefault(el => el.Status.StatusType == StatusType.Strength);
            int dmg;
            if (str == null)
            {
                dmg = 14;
            }
            else
            {
                var mult = upgradeCount == 0 ? 3 : 5;
                dmg = 14 + mult * str.Intensity;
            }
            ef.TargetEffect.InitialDamage = new List<int>() { dmg };
        }
    }
}
