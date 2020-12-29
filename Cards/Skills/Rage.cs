using System.Collections.Generic;

namespace StS
{
    public class Rage : IroncladSkillCard
    {
        public override string Name => nameof(Rage);

        public override TargetType TargetType => TargetType.Player;

        public override int CiCanCallEnergyCost(int upgradeCount) => 0;

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, long? key = null)
        {
            var amt = upgradeCount == 0 ? 3 : 5;
            ef.PlayerEffect.Status.Add(new StatusInstance(new RageStatus(), amt));
        }
    }
}
