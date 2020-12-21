using System.Collections.Generic;

namespace StS
{

    public class Dazed : StatusCard
    {
        public override string Name => nameof(Dazed);
        public override CardType CardType => CardType.Status;
        public override CardDomain CardDomain => CardDomain.Status;
        public override TargetType TargetType => TargetType.Player;
        internal override bool Ethereal(int upgradeCount) => true;
        public override int CiCanCallEnergyCost(int upgradeCount) => int.MaxValue;
        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, List<CardInstance> targets = null, Deck deck = null) { }
        public override bool Playable(List<CardInstance> hand) => false;
    }
}
