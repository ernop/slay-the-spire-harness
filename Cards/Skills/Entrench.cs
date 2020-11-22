using System.Collections.Generic;

namespace StS
{
    public class Entrench : SkillCard
    {
        public override string Name => nameof(Entrench);

        public override CharacterType CharacterType => CharacterType.IronClad;

        public override TargetType TargetType => TargetType.Player;

        public override int CiCanCallEnergyCost(int upgradeCount) => upgradeCount == 0 ? 2 : 1;

        public override bool Ethereal(int upgradeCount) => false;

        public override bool Exhausts(int upgradeCount) => false;
        public override void OtherEffects(Action action, EffectSet ef, int upgradeCount) { }

        internal override void Play(EffectSet ef, Entity source, Entity target, int upgradeCount, List<CardInstance> targets = null)
        {
            //is it as simple as this?
            ef.TargetEffect.BlockAdjustments.Add(new Progression("Entrench", (el, entity) => entity.Block));
        }
    }
}
