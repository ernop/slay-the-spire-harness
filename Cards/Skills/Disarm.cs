using System.Collections.Generic;

namespace StS
{
    public class Disarm : IroncladSkillCard
    {
        public override string Name => nameof(Disarm);

        public override TargetType TargetType => TargetType.Enemy;
        public override int CiCanCallEnergyCost(int upgradeCount) => 1;

        public override void OtherEffects(Action action, EffectSet ef, int upgradeCount) { }

        internal override void Play(EffectSet ef, Entity source, Entity target, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
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
