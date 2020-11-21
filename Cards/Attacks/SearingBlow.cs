using System.Collections.Generic;

namespace StS
{
    public class SearingBlow : AttackCard
    {

        public override string Name => nameof(SearingBlow);

        public override CharacterType CharacterType => CharacterType.IronClad;

        public override int CiCanCallEnergyCost(int upgradeCount) => 2;

        public override bool Ethereal(int upgradeCount) => false;

        public override bool Exhausts(int upgradeCount) => false;

        internal override void Play(EffectSet ef, Entity source, Entity target, int upgradeCount)
        {
            var dmg = 12 + 6 * upgradeCount;
            ef.TargetEffect.InitialDamage = new List<int>() { dmg };
        }
    }
}
