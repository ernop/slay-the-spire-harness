namespace StS
{
    public class FlameBarrier : SkillCard
    {
        public override string Name => nameof(FlameBarrier);

        public override CharacterType CharacterType => CharacterType.IronClad;

        public override TargetType TargetType => TargetType.Player;

        public override int CiCanCallEnergyCost(int upgradeCount) => 2;

        public override bool Ethereal(int upgradeCount) => false;

        public override bool Exhausts(int upgradeCount) => false;
        public override void OtherEffects(Action action, EffectSet ef, int upgradeCount) { }

        internal override void Play(EffectSet ef, Entity source, Entity target, int upgradeCount)
        {
            var blockAmount = upgradeCount == 0 ? 12 : 16;
            ef.TargetEffect.InitialBlock = blockAmount;

            var intensity = upgradeCount == 0 ? 4 : 6;
            ef.TargetEffect.Status.Add(new StatusInstance(new FlameBarrierStatus(), intensity));
        }
    }
}
