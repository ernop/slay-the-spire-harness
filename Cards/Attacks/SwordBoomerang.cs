﻿using System.Collections.Generic;

namespace StS
{
    public class SwordBoomerang : AttackCard
    {
        public override string Name => nameof(SwordBoomerang);

        public override CharacterType CharacterType => CharacterType.IronClad;

        public override int EnergyCost(int upgradeCount) => 1;

        public override bool Ethereal(int upgradeCount) => false;

        public override bool Exhausts(int upgradeCount) => false;

        internal override void Apply(EffectSet ef, Entity source, Entity target, int upgradeCount)
        {
            if (upgradeCount == 0)
            {
                ef.TargetEffect.InitialDamage = new List<int>() { 3, 3, 3 };
            }
            else
            {
                ef.TargetEffect.InitialDamage = new List<int>() { 3, 3, 3, 3 };
            }
        }
    }
}
