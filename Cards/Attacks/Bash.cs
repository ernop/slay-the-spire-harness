using System;
using System.Collections.Generic;

namespace StS
{
    public class Bash : AttackCard
    {
        public override string Name => nameof(Bash);
        public override CharacterType CharacterType => CharacterType.IronClad;
        
        public override bool Ethereal(int upgradeCount) => false;

        public override bool Exhausts(int upgradeCount) => false;
        public override int EnergyCost(int upgradeCount) => 2;

        internal override void Apply(EffectSet ef, Entity source, Entity target, int upgradeCount)
        {
            int amt;
            StatusInstance si;
            if (upgradeCount == 0)
            {
                amt = 8;
                si = new StatusInstance(new Vulnerable(), 2, int.MaxValue);
            }
            else
            {
                amt = 12;
                si = new StatusInstance(new Vulnerable(), 3, int.MaxValue);
            }


            ef.TargetEffect.InitialDamage = new List<int>() { amt };
            ef.TargetEffect.Status.Add(si);
        }
    }
}
