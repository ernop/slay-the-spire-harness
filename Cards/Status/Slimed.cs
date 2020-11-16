﻿namespace StS
{
    public class Slimed : StatusCard
    {
        public override string Name => nameof(Slimed);

        public override CharacterType CharacterType => CharacterType.Enemy;

        public override TargetType TargetType => TargetType.None;

        public override int EnergyCost(int upgradeCount) => 1;

        public override bool Ethereal(int upgradeCount) => false;

        public override bool Exhausts(int upgradeCount) => true;

        internal override void Apply(EffectSet ef, Entity source, Entity target, int upgradeCount)
        {
        }
    }
}
