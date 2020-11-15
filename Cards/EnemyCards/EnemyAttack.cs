using System.Collections.Generic;

namespace StS
{
    public class EnemyAttack : EnemyCard
    {
        public override string Name => nameof(EnemyAttack);

        public override CharacterType CharacterType => CharacterType.Enemy;

        public override bool Ethereal(int upgradeCount) => false;
        public override bool Exhausts(int upgradeCount) => false;
        public int Amount { get; set; }

        public override TargetType TargetType => TargetType.Player;

        public EnemyAttack(int amount)
        {
            Amount = amount;
        }

        internal override EffectSet Apply(Entity source, Entity target, int upgradeCount)
        {
            var ef = new EffectSet();
            ef.TargetEffect.ReceiveDamage.Add(new Progression("EnemyDamage", el => el + Amount));
            
            return ef;
        }
    }
}
