using System.Collections.Generic;
using System.Linq;

namespace StS
{
    public class Burn : StatusCard
    {
        public override string Name => nameof(Burn);

        public override CardDomain CardDomain => CardDomain.Status;

        public override TargetType TargetType => TargetType.Player;

        public override int CiCanCallEnergyCost(int upgradeCount) => int.MaxValue;

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, long? key = null) { }
        public override void LeftInHandAtEndOfTurn(IndividualEffect ie, int upgradeCount)
        {
            var dmg = upgradeCount == 0 ? 2 : 4;

            //in this context (end of player turn), there is no pattern of initialdamage/damage mods.  more like a seq of events.
            //todo: fix this to just have independent sequences of damage.
            if (ie.GetInitialDamage() == null)
            {
                ie.SetInitialDamage(dmg);
            }
            else
            {
                var old = ie.GetInitialDamage().ToList();
                old.Add(dmg);

                ie.SetInitialDamage(old.ToArray());
            }
        }
    }
}
