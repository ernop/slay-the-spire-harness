using System.Collections.Generic;

namespace StS
{
    public class Headbutt : AttackCard
    {
        public override string Name => nameof(Headbutt);

        public override CharacterType CharacterType => CharacterType.IronClad;

        public override int EnergyCost(int upgradeCount) => 1;

        public override bool Ethereal(int upgradeCount) => false;

        public override bool Exhausts(int upgradeCount) => false;
        public override void OtherEffects(Action action, EffectSet ef, int upgradeCount)
        {
            if (action == Action.Play)
            {
                ef.HandEffect = HandEffect.PullACardFromDiscardToTopOfDraw;
            }
        }

        internal override void Play(EffectSet ef, Entity source, Entity target, int upgradeCount)
        {
            int dmg;
            if (upgradeCount == 0)
            {
                dmg = 9;
            }
            else
            {
                dmg = 12;
            }
            ef.TargetEffect.InitialDamage = new List<int>() { dmg };
        }
    }
}
