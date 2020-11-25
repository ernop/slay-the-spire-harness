﻿
using System.Collections.Generic;

using static StS.Helpers;

namespace StS
{
    public class EnemyAttack : EnemyCard
    {
        public override string Name => nameof(EnemyAttack);

        public override CharacterType CharacterType => CharacterType.Enemy;
        public int Amount { get; set; }
        public int Count { get; set; }

        public override TargetType TargetType => TargetType.Player;

        public EnemyAttack(int amount, int count)
        {
            Amount = amount;
            Count = count;
        }

        internal override void Play(EffectSet ef, Entity source, Entity target, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
        {
            ef.TargetEffect.InitialDamage = Repeat(Amount, Count);
        }

        public override int CiCanCallEnergyCost(int upgradeCount) => 0;
    }
}
