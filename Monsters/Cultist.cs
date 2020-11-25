using System.Collections.Generic;

namespace StS
{
    public class Cultist : Enemy
    {
        public bool firstRound { get; set; } = true;
        public Cultist() : base(nameof(Cultist))
        { }

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
    }
}
