using System.Collections.Generic;

namespace StS
{
    public class EnemyAction
    {
        public List<StatusInstance> Buffs { get; set; }
        public EnemyAttack Attack { get; set; }
        public List<StatusInstance> PlayerStatusAttack { get; set; }

        public EnemyAction(List<StatusInstance> buff = null, EnemyAttack attack = null, List<StatusInstance> playerStatusAttack = null)
        {
            Buffs = buff;
            Attack = attack;
            PlayerStatusAttack = playerStatusAttack;
        }
    }
}
