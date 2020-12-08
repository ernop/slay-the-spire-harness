using System.Collections.Generic;
using System.Linq;

namespace StS
{
    public class GenericEnemy : Enemy, IEnemy
    {
        public GenericEnemy(int? hpMax = null, int? hp = null) : base(nameof(GenericEnemy), hpMax ?? 50, hp ?? 50) { }

        public override IEnemy Copy()
        {
            throw new System.NotImplementedException();
        }

        public override EnemyAction GetAction()
        {
            throw new System.NotImplementedException();
        }
    }
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


        public override IEnemy Copy()
        {
            var sis = StatusInstances.Select(el => el.Copy());
            return new Cultist(0, 0)
            {
                Name = Name,
                Block = Block,
                StatusInstances = sis.ToList(),
                FirstRound = FirstRound,
                HP = HP,
                HPMax = HPMax
            };
        }
    }
}
