using System.Collections.Generic;

namespace StS
{
    public class Strike : IroncladAttackCard
    {
        public override string Name => nameof(Strike);
        public override int CiCanCallEnergyCost(int upgradeCount) => 1;

        internal override void Play(EffectSet ef, IEntity source, IEntity target, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
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
