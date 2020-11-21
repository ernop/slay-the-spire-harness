using System.Collections.Generic;

namespace StS
{
    public class Clothesline : AttackCard
    {
        public override string Name => nameof(Clothesline);

        public override CharacterType CharacterType => CharacterType.IronClad;

        public override int CiCanCallEnergyCost(int upgradeCount) => 2;

        public override bool Ethereal(int upgradeCount) => false;

        public override bool Exhausts(int upgradeCount) => false;

        internal override void Play(EffectSet ef, Entity source, Entity target, int upgradeCount)
        {
            int dmg;
            StatusInstance si;
            if (upgradeCount == 0)
            {
                dmg = 12;
                si= new StatusInstance(new Weak(), 2);
            }
            else
            {
                dmg = 14;
                si = new StatusInstance(new Weak(), 3);
            }
            ef.TargetEffect.InitialDamage = new List<int>() { dmg };
            ef.TargetEffect.Status.Add(si);
        }
    }
}
