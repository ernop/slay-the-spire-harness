using System.Collections.Generic;

namespace StS
{
    public class Disarm : SkillCard
    {
        public override string Name => nameof(Disarm);

        public override CharacterType CharacterType => CharacterType.IronClad;

        public override TargetType TargetType => TargetType.Enemy;

        public override bool Ethereal(int upgradeCount)
        {
            return false;
        }

        public override bool Exhausts(int upgradeCount)
        {
            return true;
        }

        internal override EffectSet Apply(Entity source, Entity target, int upgradeCount)
        {
            int amt = 0;
            if (upgradeCount == 0)
            {
                amt = -2;
            }
            else
            {
                amt = -3;
            }
            var ef = new EffectSet();
            ef.TargetEffect.Status.Add(new StatusInstance(new Strength(), int.MaxValue, amt));
            return ef;
        }
    }
}
