﻿using System.Collections.Generic;

namespace StS
{
    public class Slimed : StatusCard
    {
        public override string Name => nameof(Slimed);
        public override CharacterType CharacterType => CharacterType.Enemy;
        public override TargetType TargetType => TargetType.Player;
        public override int CiCanCallEnergyCost(int upgradeCount) => 1;
        internal override bool Exhausts(int upgradeCount) => true;
        public override void OtherEffects(Action action, EffectSet ef, int upgradeCount) { }
        internal override void Play(EffectSet ef, Entity source, Entity target, int upgradeCount, List<CardInstance> targets = null, Deck deck = null) { }
    }
}
