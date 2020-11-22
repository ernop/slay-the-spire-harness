using System.Collections.Generic;

namespace StS
{
    public class Uppercut : AttackCard
    {
        public override string Name => nameof(Uppercut);

        public override CharacterType CharacterType => CharacterType.IronClad;

        public override bool Ethereal(int upgradeCount) => false;

        public override bool Exhausts(int upgradeCount) => false;
        public override int CiCanCallEnergyCost(int upgradeCount) => 2;

        internal override void Play(EffectSet ef, Entity source, Entity target, int upgradeCount, List<CardInstance> targets = null)
        {
            int weak;
            int vuln;
            if (upgradeCount == 0)
            {
                weak = 1;
                vuln = 1;
            }
            else
            {
                weak = 2;
                vuln = 2;
            }
            ef.TargetEffect.InitialDamage = new List<int>() { 13 };
            ef.TargetEffect.Status.Add(new StatusInstance(new Vulnerable(), vuln));
            ef.TargetEffect.Status.Add(new StatusInstance(new Weak(), weak));
        }
    }
}
