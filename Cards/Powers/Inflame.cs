using System;
using System.Collections.Generic;
using System.Text;

namespace StS
{
    public class Inflame : PowerCard
    {
        public override string Name => nameof(Inflame);

        public override CharacterType CharacterType => CharacterType.IronClad;

        public override CardType CardType => CardType.Power;
        public override TargetType TargetType => TargetType.Player;
        public override bool Ethereal(int upgradeCount) => false;
        public override bool Exhausts(int upgradeCount) => false;

        internal override EffectSet Apply(Entity source, Entity target, int upgradeCount)
        {
            var amt = 0;
            if (upgradeCount == 0)
            {
                amt = 2;
            }
            else
            {
                amt = 3;
            }

            var ef = new EffectSet();
            ef.TargetEffect.Status.Add(new StatusInstance(new Strength(), int.MaxValue, amt));

            return ef;
        }
    }
}
