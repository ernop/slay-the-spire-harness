using System.Collections.Generic;

namespace StS
{
    public class Inflame : IroncladPowerCard
    {
        public override string Name => nameof(Inflame);
        public override TargetType TargetType => TargetType.Player;
        public override int CiCanCallEnergyCost(int upgradeCount) => 1;

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, int? key = null)
        {
            int amt = upgradeCount == 0 ? 2 : 3;
            ef.PlayerEffect.Status.Add(new StatusInstance(new Strength(), amt));
        }
    }
}
