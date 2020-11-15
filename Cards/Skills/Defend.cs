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

        internal override EffectSet Apply(Entity source, Entity target, int upgradeCount)
        {
            int amount;
            if (upgradeCount == 0)
            {
                amount = 5;
            }
            else
            {
                amount = 6;
            }

            var ef = new EffectSet();
            ef.TargetEffect.GainBlock.Add(new Progression("Defend", (_) => amount));
            return ef;
        }
    }
}
