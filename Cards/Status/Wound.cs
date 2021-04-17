using System.Collections.Generic;

namespace StS
{
    public class Wound : StatusCard
    {
        public override string Name => nameof(Wound);

        public override CardDomain CardDomain => CardDomain.Status;

        public override TargetType TargetType => TargetType.Player;

        public override EnergyCostInt CiCanCallEnergyCost(int upgradeCount) => new EnergyCostInt(int.MaxValue);
        public override bool Playable(IList<CardInstance> hand) => false;

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, long? key = null)
        {
            throw new System.NotImplementedException();
        }
    }
}
