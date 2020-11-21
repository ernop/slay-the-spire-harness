using System.Collections.Generic;
using System.Linq;
using static StS.Helpers;

namespace StS
{
    public class EnemyAttack : EnemyCard
    {
        public override string Name => nameof(EnemyAttack);

        public override CharacterType CharacterType => CharacterType.Enemy;

        public override bool Ethereal(int upgradeCount) => false;
        public override bool Exhausts(int upgradeCount) => false;
        public int Amount { get; set; }
        public int Count { get; set; }

        public override TargetType TargetType => TargetType.Player;

        public EnemyAttack(int amount, int count)
        {
            Amount = amount;
            Count = count;
        }

        internal override void Play(EffectSet ef, Entity source, Entity target, int upgradeCount)
        {
            ef.TargetEffect.InitialDamage = Repeat(Amount, Count);
        }

        public override int CiCanCallEnergyCost(int upgradeCount)
        {
            return 0;
        }
    }
}
