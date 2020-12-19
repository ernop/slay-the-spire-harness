using System.Collections.Generic;

namespace StS
{
    public class AscendersBane : StatusCard
    {
        public override string Name => nameof(AscendersBane);
        public override CardDomain CardDomain => CardDomain.Status;
        public override TargetType TargetType => TargetType.Player;
        public override int CiCanCallEnergyCost(int upgradeCount) => int.MaxValue;
        internal override bool Ethereal(int upgradeCount) => true;
        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, List<CardInstance> targets = null, Deck deck = null) { }
    }
}
