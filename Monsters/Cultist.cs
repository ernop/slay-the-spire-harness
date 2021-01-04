﻿using System.Collections.Generic;

namespace StS
{



    public class Cultist : Enemy
    {
        public Cultist(int? hp = null, int? hpMax = null) : base(nameof(Cultist), hp, hpMax) { }

        public override FightAction GetAction(int turn)
        {
            if (turn == 1)
            {
                var res = new FightAction(FightActionEnum.EnemyMove, hadRandomEffects: true, card: new CardInstance(new EnemyCard(targetType: TargetType.Enemy, buffs: new List<StatusInstance>() { new StatusInstance(new Feather(), 3) }), 0), key: 1);
                return res;
            }
            else
            {
                var res = new FightAction(FightActionEnum.EnemyMove, hadRandomEffects: true, card: new CardInstance(new EnemyCard(targetType: TargetType.Player, 6, 1), 0), key: 1);
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
