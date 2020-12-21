using System.Collections.Generic;

namespace StS
{
    public class Cultist : Enemy, IEnemy
    {
        public Cultist(int? hp = null, int ? hpMax = null) : base(nameof(Cultist), hp, hpMax) { }

        public override FightAction GetAction(int turn)
        {
            if (turn == 1)
            {
                var res = new FightAction(FightActionEnum.EnemyMove, card: new CardInstance(new EnemyCard(buffs: new List<StatusInstance>() { new StatusInstance(new Feather(), 3) }),0));
                return res;
            }
            else
            {
                var res = new FightAction(FightActionEnum.EnemyMove, card: new CardInstance(new EnemyCard(3, 1),0));
                return res;
            }
        }

        /// <summary>
        /// Only need to copy the specific fields.
        /// </summary>
        public override IEnemy Copy()
        {
            var res = new Cultist();
            CopyEntity(res);
            return res;
        }
    }
}
