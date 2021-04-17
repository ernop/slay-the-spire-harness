using System.Collections.Generic;

namespace StS
{
    public class AscendersBane : StatusCard
    {
        public override string Name => nameof(AscendersBane);
        public override CardDomain CardDomain => CardDomain.Status;
        public override TargetType TargetType => TargetType.Player;
        public override EnergyCostInt CiCanCallEnergyCost(int upgradeCount) => new EnergyCostInt(int.MaxValue);
        internal override bool Ethereal(int upgradeCount) => true;
        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, long? key = null) { }
    }
}
