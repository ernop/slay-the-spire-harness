using System.Collections.Generic;

namespace StS
{
    public class Cultist : Enemy, IEnemy
    {
        public bool FirstRound { get; set; } = true;
        public Cultist(int? hpMax = null, int? hp = null) : base(nameof(Cultist), hpMax, hp) { }

        public override EnemyAction GetAction()
        {
            if (FirstRound)
            {
                FirstRound = false;
                var res = new EnemyAction(new List<StatusInstance>() { new StatusInstance(new Feather(), 3) }, null, null);
                return res;
            }
            else
            {
                var res = new EnemyAction(null, new EnemyAttack(3, 1), null);
                return res;
            }
        }


        /// <summary>
        /// Only need to copy the specific fields.
        /// </summary>
        public override IEnemy Copy()
        {
            var res = new Cultist()
            {
                FirstRound = FirstRound,
            };
            CopyEntity(res);
            return res;
        }
    }
}
