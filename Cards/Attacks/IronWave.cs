using System.Collections.Generic;

namespace StS
{
    public class IronWave : AttackCard
    {
        public override string Name => nameof(IronWave);

        public override CharacterType CharacterType => CharacterType.IronClad;

        public override int CiCanCallEnergyCost(int upgradeCount) => 1;

        public override bool Ethereal(int upgradeCount) => false;

        public override bool Exhausts(int upgradeCount) => false;

        internal override void Play(EffectSet ef, Entity source, Entity target, int upgradeCount)
        {
            int dmg;
            int block;
            if (upgradeCount == 0)
            {
                dmg = 5;
                block = 5;
            }
            else
            {
                dmg = 7;
                block = 7;
            }

            ef.TargetEffect.InitialDamage = new List<int>() { dmg };
            ef.SourceEffect.InitialBlock = block;
        }
    }
}
