namespace StS
{
    public class ShockWave : SkillCard
    {
        public override string Name => nameof(ShockWave);

        public override CharacterType CharacterType => CharacterType.IronClad;

        public override TargetType TargetType => TargetType.Enemy;

        public override int EnergyCost(int upgradeCount) => 2;

        public override bool Ethereal(int upgradeCount) => false;

        public override bool Exhausts(int upgradeCount) => true;

        internal override void Apply(EffectSet ef, Entity source, Entity target, int upgradeCount)
        {
            var vuln = upgradeCount == 0 ? 3 : 5;
            var weak = upgradeCount == 0 ? 3 : 5;
            ef.TargetEffect.Status.Add(new StatusInstance(new Vulnerable(), vuln));
            ef.TargetEffect.Status.Add(new StatusInstance(new Weak(), weak));
        }
    }
}
