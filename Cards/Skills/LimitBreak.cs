using System.Collections.Generic;
using System.Linq;

namespace StS
{
    public class LimitBreak : IroncladSkillCard
    {
        public override string Name => nameof(LimitBreak);
        public override TargetType TargetType => TargetType.Player;
        internal override bool Exhausts(int upgradeCount) => upgradeCount == 0 ? true : false;
        public override int CiCanCallEnergyCost(int upgradeCount) => 1;

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, int? key = null)
        {
            var exi = player.StatusInstances.SingleOrDefault(el => el.Status.StatusType == StatusType.Strength);
            if (exi == null)
            {

            }
            else
            {
                //we could just double this in place but cleaner to reapply the same status
                var statusCopy = new StatusInstance(exi.Status, exi.Intensity);
                ef.PlayerEffect.Status.Add(statusCopy);
            }
        }
    }
}
