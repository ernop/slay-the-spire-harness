using System.Collections.Generic;

namespace StS
{
    public class EnemyCard : Card
    {
        public override string Name => nameof(EnemyCard);
        public override CardType CardType { get; }
        public override CardDomain CardDomain => CardDomain.Enemy;
        public int? Amount { get; set; }
        public int? Count { get; set; }

        public override TargetType TargetType { get; }
        public List<StatusInstance> Buffs { get; set; }
        public List<StatusInstance> PlayerStatusAttack { get; set; }
        public override EnergyCostInt CiCanCallEnergyCost(int upgradeCount) => new EnergyCostInt(0);
        public EnemyCard(TargetType targetType, int? amount = null, int? count = null, List<StatusInstance> buffs = null, List<StatusInstance> playerStatusAttack = null)
        {
            TargetType = targetType;
            Amount = amount;
            Count = count;
            Buffs = buffs;
            PlayerStatusAttack = playerStatusAttack;
            if (amount != null)
            {
                CardType = CardType.Attack;
            }
            else
            {
                CardType = CardType.Skill;
            }
        }

        internal override void Play(EffectSet ef, Player player, IEnemy enemy, int upgradeCount, IList<CardInstance> targets = null, Deck deck = null, long? key = null)
        {
            if (Amount.HasValue)
            {
                ef.PlayerEffect.SetInitialDamage(Helpers.Repeat(Amount.Value, Count.Value).ToArray());
            }
            if (PlayerStatusAttack != null)
            {
                ef.PlayerEffect.Status.AddRange(PlayerStatusAttack);
            }
            if (Buffs != null)
            {
                ef.EnemyEffect.Status.AddRange(Buffs);
            }
        }

        public override string ToString()
        {
            var p = PlayerStatusAttack == null ? "" : " P:" + string.Join(',', PlayerStatusAttack);
            var b = Buffs == null ? "" : " B:" + string.Join(',', Buffs);
            var a = Amount == null ? "" : $" amt:{Amount}x{Count}";
            return $"{nameof(EnemyCard)}{a}{b}{p}";
        }
    }
}
