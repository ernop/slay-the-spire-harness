using System;

namespace StS
{
    public class Enemy : Entity
    {
        public Enemy(string name, GameContext context, int hpMax, int hp) : base(name, context, EntityType.Enemy, hpMax, hp)
        {
        }

        public virtual EnemyAction GetAction()
        {
            throw new NotImplementedException();
        }
    }
}
