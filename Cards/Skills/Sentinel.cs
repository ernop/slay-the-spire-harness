using System.Collections.Generic;

namespace StS
{
    public class Sentinel : IroncladSkillCard
    {
        public override string Name => nameof(Sentinel);

        public override TargetType TargetType => TargetType.Player;

        public override int CiCanCallEnergyCost(int upgradeCount) => 1;
        public override void OtherAction(Action action, EffectSet ef, int upgradeCount)
        {
            if (action == Action.Exhaust)
            {
                ef.PlayerEnergy += upgradeCount == 0 ? 2 : 3;
            }
        }

        internal override void Play(EffectSet ef, IEntity source, IEntity target, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
        {

            int amount;
            if (upgradeCount == 0)
            {
                amount = 5;
            }
            else
            {
                amount = 8;
            }
            ef.TargetEffect.InitialBlock = amount;
        }
    }
}
