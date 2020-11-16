using System.Collections.Generic;

namespace StS
{
    public class BodySlam : AttackCard
    {
        public override string Name => nameof(BodySlam);

        public override CharacterType CharacterType => CharacterType.IronClad;

        public override int EnergyCost(int upgradeCount) => upgradeCount == 0 ? 1 : 0;

        public override bool Ethereal(int upgradeCount) => false;

        public override bool Exhausts(int upgradeCount) => false;

        internal override void Apply(EffectSet ef, Entity source, Entity target, int upgradeCount)
        {
            ef.TargetEffect.InitialDamage = new List<int>() { source.Block };
        }
    }
}
