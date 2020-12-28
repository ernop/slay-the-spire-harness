using System.Collections.Generic;

namespace StS
{
    public class GenericEnemy : Enemy, IEnemy
    {
        public int Amount { get; set; }
        public int Count { get; set; }

        /// <summary>
        /// Defaults to no attack.
        /// </summary>
        public GenericEnemy(int amount = 0, int count = 0, int? hp = null, int? hpMax = null, List<StatusInstance> statuses = null) : base(nameof(GenericEnemy), hp, hpMax)
        {
            Amount = amount;
            Count = count;
            if (statuses != null)
            {
                StatusInstances = statuses;
            }
        }

        public override FightAction GetAction(int turnNumber)
        {
            return new FightAction(FightActionEnum.EnemyMove, card: new CardInstance(new EnemyCard(Amount, Count), 0), hadRandomEffects: true, key:1);
        }

        public override IEnemy Copy()
        {
            var res = new GenericEnemy(Amount, Count, HP, HPMax);
            CopyEntity(res);
            return res;
        }
    }
}
