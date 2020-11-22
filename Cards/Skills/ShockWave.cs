using System.Collections.Generic;

namespace StS
{
    public class Shockwave : SkillCard
    {
        public override string Name => nameof(Shockwave);

        public override CharacterType CharacterType => CharacterType.IronClad;

        public override TargetType TargetType => TargetType.Enemy;

        public override int CiCanCallEnergyCost(int upgradeCount) => 2;

        public override bool Ethereal(int upgradeCount) => false;

        public override bool Exhausts(int upgradeCount) => true;
        public override void OtherEffects(Action action, EffectSet ef, int upgradeCount) { }

        internal override void Play(EffectSet ef, Entity source, Entity target, int upgradeCount, List<CardInstance> targets = null)
        {
            var vuln = upgradeCount == 0 ? 3 : 5;
            var weak = upgradeCount == 0 ? 3 : 5;
            ef.TargetEffect.Status.Add(new StatusInstance(new Vulnerable(), vuln));
            ef.TargetEffect.Status.Add(new StatusInstance(new Weak(), weak));
        }
    }
}
