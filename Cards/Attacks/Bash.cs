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

        internal override EffectSet Apply(Entity source, Entity target, int upgradeCount)
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


            var ef = new EffectSet();
            ef.TargetEffect.ReceiveDamage.Add(new Progression("Bash", (_) => amt));
            ef.TargetEffect.Status.Add(si);

            return ef;
        }
    }
}
