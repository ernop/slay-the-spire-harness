using System.Collections.Generic;

namespace StS
{
    public class Strike : AttackCard
    {
        public override string Name => nameof(Strike);

        public override CharacterType CharacterType => CharacterType.IronClad;

        public override bool Ethereal(int upgradeCount) => false;

        public override bool Exhausts(int upgradeCount) => false;
        public override int CiCanCallEnergyCost(int upgradeCount) => 1;
        internal override void Play(EffectSet ef, Entity source, Entity target, int upgradeCount, List<CardInstance> targets = null)
        {
            int amount;
            if (upgradeCount == 0)
            {
                amount = 6;
            }
            else
            {
                amount = 9;
            }

            ef.TargetEffect.InitialDamage = new List<int>() { amount };
        }
    }
}
