using System.Collections.Generic;

namespace StS
{
    public class Strike : AttackCard
    {
        public override string Name => nameof(Strike);

        public override CharacterType CharacterType => CharacterType.IronClad;

        public override bool Ethereal(int upgradeCount) => false;

        public override bool Exhausts(int upgradeCount) => false;
        internal override EffectSet Apply(Entity source, Entity target, int upgradeCount)
        {
            int amount;
            if (upgradeCount == 0)
            {
                amount = 6;
            }
            else
            {
                amount = 9;
            }

            var ef = new EffectSet();
            ef.TargetEffect.ReceiveDamage.Add(new Progression("Strike", (_) => amount));
            return ef;
        }
    }
}
