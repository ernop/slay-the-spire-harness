using System.Collections.Generic;

namespace StS
{
    public class Disarm : SkillCard
    {
        public override string Name => nameof(Disarm);

        public override CharacterType CharacterType => CharacterType.IronClad;

        public override TargetType TargetType => TargetType.Enemy;
        public override int EnergyCost(int upgradeCount) => 1;

        public override bool Ethereal(int upgradeCount) => false;

        public override bool Exhausts(int upgradeCount) => true;

        internal override void Apply(EffectSet ef, Entity source, Entity target, int upgradeCount)
        {
            int amt;
            if (upgradeCount == 0)
            {
                amt = -2;
            }
            else
            {
                amt = -3;
            }
            
            ef.TargetEffect.Status.Add(new StatusInstance(new Strength(), amt));
        }
    }
}
