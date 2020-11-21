using System.Collections.Generic;

namespace StS
{

    public class Defend : SkillCard
    {
        public override string Name => nameof(Defend);
        public override CharacterType CharacterType => CharacterType.IronClad;

        public override TargetType TargetType => TargetType.Player;

        public override bool Ethereal(int upgradeCount) => false;
        public override bool Exhausts(int upgradeCount) => false;
        public override int CiCanCallEnergyCost(int upgradeCount) => 1;
        public override void OtherEffects(Action action, EffectSet ef, int upgradeCount) { }

        internal override void Play(EffectSet ef, Entity source, Entity target, int upgradeCount)
        {
            int amount;
            if (upgradeCount == 0)
            {
                amount = 5;
            }
            else
            {
                amount = 8;
            }

            ef.TargetEffect.InitialBlock = amount;
        }
    }
}
