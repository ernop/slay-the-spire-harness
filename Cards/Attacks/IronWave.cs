using System.Collections.Generic;

namespace StS
{
    public class IronWave : AttackCard
    {
        public override string Name => nameof(IronWave);

        public override CharacterType CharacterType => CharacterType.IronClad;

        public override bool Ethereal(int upgradeCount) => false;

        public override bool Exhausts(int upgradeCount) => false;

        internal override EffectSet Apply(Entity source, Entity target, int upgradeCount)
        {
            int dmg;
            int block;
            if (upgradeCount == 0)
            {
                dmg = 5;
                block = 5;
            }
            else
            {
                dmg = 7;
                block = 7;
            }

            var ef = new EffectSet();
            ef.TargetEffect.ReceiveDamage.Add(new Progression("IronWaveDamage", (_) => dmg));
            ef.SourceEffect.GainBlock.Add(new Progression("IronWaveBlock", (_) => block));
            return ef;
        }
    }
}
