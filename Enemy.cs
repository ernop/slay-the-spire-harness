using System;

namespace StS
{
    public class Enemy : Entity
    {
        public Enemy(string name, int hpMax, int hp) : base(name, EntityType.Enemy, hpMax, hp)
        {
        }

        public virtual EnemyAction GetAction()
        {
            throw new NotImplementedException();
        }
    }
}
