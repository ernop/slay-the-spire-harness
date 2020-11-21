using System;
using System.Collections.Generic;

namespace StS
{
    public class Clash : AttackCard
    {
        public override string Name => nameof(Clash);

        public override CharacterType CharacterType => CharacterType.IronClad;

        public override int CiCanCallEnergyCost(int upgradeCount) => 0;

        public override bool Ethereal(int upgradeCount) => false;

        public override bool Exhausts(int upgradeCount) => false;

        public override void OtherEffects(Action action, EffectSet ef, int upgradeCount)
        {
            if (action == Action.AttemptPlay)
            {
                //ef.HandEffect = HandEffect.CannotPlay;
                throw new NotImplementedException();
            }
        }

        internal override void Play(EffectSet ef, Entity source, Entity target, int upgradeCount)
        {
            var dmg = upgradeCount == 0 ? 14 : 18;
            ef.TargetEffect.InitialDamage = new List<int>() { dmg };
        }
    }
}
