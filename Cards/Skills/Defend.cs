using System.Collections.Generic;

namespace StS
{
    public class Defend : IroncladSkillCard
    {
        public override string Name => nameof(Defend);

        public override TargetType TargetType => TargetType.Player;

        public override int CiCanCallEnergyCost(int upgradeCount) => 1;

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null)
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

            ef.PlayerEffect.InitialBlock = amount;
        }
    }
}
