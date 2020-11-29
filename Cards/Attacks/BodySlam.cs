using System.Collections.Generic;

namespace StS
{
    public class BodySlam : IroncladAttackCard
    {
        public override string Name => nameof(BodySlam);


        public override int CiCanCallEnergyCost(int upgradeCount) => upgradeCount == 0 ? 1 : 0;

        internal override void Play(EffectSet ef, IEntity source, IEntity target, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
        {
            ef.TargetEffect.InitialDamage = new List<int>() { source.Block };
        }
    }
}
