using System.Collections.Generic;

namespace StS
{

    public class Footwork : PowerCard
    {
        public override string Name => nameof(Footwork);

        public override CharacterType CharacterType => CharacterType.Silent;

        public override CardType CardType => CardType.Power;
        public override bool Ethereal(int upgradeCount) => false;
        public override bool Exhausts(int upgradeCount) => false;
        internal override EffectSet Apply(Entity source, Entity target, int upgradeCount)
        {
            var ef = new EffectSet();
            int amt;
            if (upgradeCount == 0)
            {
                amt = 2;
            }
            else
            {
                amt = 3;
            }

            ef.TargetEffect.Status.Add(new StatusInstance(new Dexterity(), int.MaxValue, amt));
            return ef;
        }
    }
}
