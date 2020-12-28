using System.Collections.Generic;

namespace StS
{
    public class Clumsy : CurseCard
    {
        public override string Name => nameof(Clumsy);
        public override CardDomain CardDomain => CardDomain.Curse;
        public override int CiCanCallEnergyCost(int upgradeCount) => int.MaxValue;
        internal override bool Ethereal(int upgradeCount) => true;
        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, int? key = null)
        {
            throw new System.NotImplementedException();
        }
    }
}
