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
        public override int EnergyCost(int upgradeCount) => 1;

        internal override void Play(EffectSet ef, Entity source, Entity target, int upgradeCount)
        {
            int amt;
            if (upgradeCount == 0)
            {
                amt = 2;
            }
            else
            {
                amt = 3;
            }

            ef.TargetEffect.Status.Add(new StatusInstance(new Strength(), amt));
        }
    }
}
