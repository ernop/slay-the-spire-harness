using System.Collections.Generic;

namespace StS
{
    public class Wound : StatusCard
    {
        public override string Name => nameof(Wound);

        public override CardDomain CardDomain => CardDomain.Status;

        public override TargetType TargetType => TargetType.Player;

        public override int CiCanCallEnergyCost(int upgradeCount) => int.MaxValue;
        public override bool Playable(IList<CardInstance> hand) => false;

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null)
        {
            throw new System.NotImplementedException();
        }
    }
}
