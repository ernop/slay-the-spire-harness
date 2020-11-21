using System.Collections.Generic;
using System.Linq;

namespace StS
{
    public class LimitBreak : SkillCard
    {
        public override string Name => "Limit Break";

        public override CharacterType CharacterType => CharacterType.IronClad;

        public override CardType CardType => CardType.Skill;
        public override TargetType TargetType => TargetType.Player;

        public override bool Ethereal(int upgradeCount) => false;

        public override bool Exhausts(int upgradeCount) => upgradeCount==0 ? true : false;
        public override int EnergyCost(int upgradeCount) => 1;
        public override void OtherEffects(Action action, EffectSet ef, int upgradeCount) { }

        internal override void Play(EffectSet ef, Entity source, Entity target, int upgradeCount)
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
