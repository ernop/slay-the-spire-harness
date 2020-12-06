
using System.Collections.Generic;

using static StS.Helpers;

namespace StS
{

    public class EnemyAttack : EnemyCard
    {
        public override string Name => nameof(EnemyAttack);
        public override CardType CardType => CardType.Attack;

        public override CharacterType CharacterType => CharacterType.Enemy;
        public int Amount { get; set; }
        public int Count { get; set; }

        public override TargetType TargetType => TargetType.Player;

        public EnemyAttack(int amount, int count)
        {
            Amount = amount;
            Count = count;
        }

        internal override void Play(EffectSet ef, IEntity source, IEntity target, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
        {
            ef.TargetEffect.InitialDamage = Repeat(Amount, Count);
        }

        public override int CiCanCallEnergyCost(int upgradeCount) => 0;
    }
}
