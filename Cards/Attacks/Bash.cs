using System.Collections.Generic;

namespace StS
{
    public class Bash : IroncladAttackCard
    {
        public override string Name => nameof(Bash);
        public override int CiCanCallEnergyCost(int upgradeCount) => 2;

        internal override void Play(EffectSet ef, IEntity source, IEntity target, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
        {
            int amt;
            StatusInstance si;
            if (upgradeCount == 0)
            {
                amt = 8;
                si = new StatusInstance(new Vulnerable(), 2);
            }
            else
            {
                amt = 10;
                si = new StatusInstance(new Vulnerable(), 3);
            }


            ef.TargetEffect.InitialDamage = new List<int>() { amt };
            ef.TargetEffect.Status.Add(si);
        }
    }
}
