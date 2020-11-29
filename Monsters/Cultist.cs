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
        public bool firstRound { get; set; } = true;
        public Cultist() : base(nameof(Cultist)) { }

        public override EnemyAction GetAction()
        {
            if (firstRound)
            {
                firstRound = false;
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
            return new Cultist
            {
                Name = Name,
                HP = HP,
                HPMax = HPMax,
                Block = Block,
                StatusInstances = sis.ToList()
            };
        }
    }
}
