using System.Collections.Generic;

namespace StS
{
    public class Disarm : IroncladSkillCard
    {
        public override string Name => nameof(Disarm);

        public override TargetType TargetType => TargetType.Enemy;
        public override int CiCanCallEnergyCost(int upgradeCount) => 1;
        internal override bool Exhausts(int upgradeCount) => true;

        internal override void Play(EffectSet ef, IEntity source, IEntity target, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
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
