using System.Collections.Generic;

namespace StS
{
    public class Uppercut : IroncladAttackCard
    {
        public override string Name => nameof(Uppercut);
        public override int CiCanCallEnergyCost(int upgradeCount) => 2;

        internal override void Play(EffectSet ef, IEntity source, IEntity target, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
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
