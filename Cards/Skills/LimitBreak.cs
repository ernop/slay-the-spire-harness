using System.Collections.Generic;
using System.Linq;

namespace StS
{
    public class LimitBreak : IroncladSkillCard
    {
        public override string Name => "Limit Break";


        public override CardType CardType => CardType.Skill;
        public override TargetType TargetType => TargetType.Player;
        public override bool Exhausts(int upgradeCount) => upgradeCount==0 ? true : false;
        public override int CiCanCallEnergyCost(int upgradeCount) => 1;
     
        internal override void Play(EffectSet ef, Entity source, Entity target, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
        {
            var exi = target.StatusInstances.SingleOrDefault(el => el.Status.StatusType == StatusType.Strength);
            if (exi == null)
            {

            }
            else
            {
                //we could just double this in place but cleaner to reapply the same status
                var statusCopy = new StatusInstance(exi.Status, exi.Intensity);
                ef.TargetEffect.Status.Add(statusCopy);
            }
        }
    }
}
