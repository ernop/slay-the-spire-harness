﻿using System.Collections.Generic;

namespace StS
{
    public class Dazed : StatusCard
    {
        public override string Name => nameof(Dazed);

        public override CharacterType CharacterType => CharacterType.Colorless;

        public override TargetType TargetType => TargetType.Player;
        internal override bool Ethereal(int upgradeCount) => true;
        public override int CiCanCallEnergyCost(int upgradeCount) => int.MaxValue;
        internal override void Play(EffectSet ef, Entity source, Entity target, int upgradeCount, List<CardInstance> targets = null, Deck deck = null) { }
    }
}
